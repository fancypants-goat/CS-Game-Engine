using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using OpenTK.Mathematics;

namespace Engine;

public struct SceneData
{
    public string Name { get; set; }
    public string Path { get; set; }

    public List<Entity> Entities { get; set; }
    public List<IDrawable> Drawables { get; set; }
    public Camera ActiveCamera { get; set; }
}


internal static class SceneLoader
{
    internal static SceneData LoadSceneData(string path)
    {
        var data = new SceneData
        {
            Path = path
        };
        
        // global scene data
        var activeCameraId = "";
        
        // current entity data
        Entity currentEntity = null;
        
        

        var source = File.ReadAllText(path);
        Debug.Log($"Loading scene {path}");

        var i = 0;
        foreach (var line in source.Split('\n'))
        {
            i++;
            var l = line.Trim();
            if (l.StartsWith('#') || string.IsNullOrWhiteSpace(l)) continue;
            
            Debug.Log("Decoding line", i, ":", l);
            
            // find the command and rest using RegEx
            const string commandPattern = @"\A(?<command>\w+) \s* (?<rest>.*)";
            var match = Regex.Match(l, commandPattern, RegexOptions.IgnorePatternWhitespace);
            
            if (!match.Success)
            {
                Debug.Log("Failed to decode line");
                continue;
            }
            
            var command = match.Groups["command"].Value;
            const string argPattern = """(?<arg>"[^"]*" | \w+\([^\)]*\) | [^\s"()<>]+)""";
            var matches = Regex.Matches(match.Groups["rest"].Value, argPattern, RegexOptions.IgnorePatternWhitespace);
            var args = matches.Select(m => m.Groups["arg"].Value).ToArray();

            switch (command)
            {
                // global scene data
                case "scene":
                    data.Name = DecodeString(args[0]);
                    break;
                case "camera":
                    activeCameraId = DecodeString(args[0]);
                    break;
                // entity scene data
                case "entity":
                    if (currentEntity != null)
                        data.Entities.Add(currentEntity);
                    
                    currentEntity = new Entity(DecodeString(args[0]));
                    break;
                case "add":
                    var name = DecodeString(args[0]);
                    if (!ComponentRegistry.GetComponentType(name, out Type? type))
                    {
                        Debug.LogError("Unknown component type:", name);
                        continue;
                    }
                    
                    Parameter parentParameter = new Parameter(currentEntity, typeof(Entity));
                    List<Parameter> parameters = [parentParameter];
                    for (int j = 1; j < args.Length; j++)
                    {
                        parameters.Add(DecodeParameter(args[j]));
                    }
                    var c = ComponentRegistry.Create(type, parameters);

                    if (type.IsSubclassOf(typeof(IDrawable)))
                        data.Drawables.Add((IDrawable)c);

                    if (type == typeof(Camera) && currentEntity.Id == activeCameraId)
                        data.ActiveCamera = (Camera)c;
                    
                    currentEntity.AddComponent(c);
                    break;
            }
        }

        return data;
    }


    // --------------------------
    // Basic decoding helpers
    // --------------------------
    private static string DecodeString(string v)
    {
        return v.Trim().Trim('"');
    }

    private static Mesh DecodeMesh(string arg)
    {
        return (Mesh)Resources.GetMesh(DecodeString(arg));
    }

    private static Texture DecodeTexture(params string[] args)
    {
        string path = DecodeString(args[0]);
        bool enabled = args.Length > 1 && bool.TryParse(args[1], out bool b) ? b : true;
        return new Texture(path, enabled);
    }

    private static Shader DecodeShader(params string[] args)
    {
        if (args.Length == 2 && !string.IsNullOrEmpty(args[1]))
            return new Shader(DecodeString(args[0]), DecodeString(args[1]));

        return Resources.GetShader(DecodeString(args[0]));
    }

    private static Vector3 DecodeVector3(string arg)
    {
        // Expecting: vec3(x), vec3(x, y, z)
        var match = Regex.Match(arg, @"vec3\(([^)]*)\)");
        if (!match.Success)
            throw new ArgumentException($"Invalid vec3 format: {arg}");

        var parts = match.Groups[1].Value
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => float.Parse(p.Trim()))
            .ToArray();

        return parts.Length switch
        {
            1 => new Vector3(parts[0], 0, 0),
            3 => new Vector3(parts[0], parts[1], parts[2]),
            _ => throw new ArgumentException($"vec3 must have 1 or 3 values: {arg}")
        };
    }

    // --------------------------
    // Main parameter decoder
    // --------------------------
    public static Parameter DecodeParameter(string v)
    {
        v = v.Trim();

        // String literal
        if (v.StartsWith('"') && v.EndsWith('"'))
            return new Parameter(DecodeString(v), typeof(string));

        // Integer literal
        if (int.TryParse(v, out int i))
            return new Parameter(i, typeof(int));

        // Function-style argument: mesh(...), texture(...), shader(...), vec3(...)
        var match = Regex.Match(v, @"(?<name>\w+)\((?<args>[^\)]*)\)");
        if (match.Success)
        {
            string name = match.Groups["name"].Value;
            string[] args = match.Groups["args"].Value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim()).ToArray();

            return name switch
            {
                "mesh" => new Parameter(DecodeMesh(args[0]), typeof(Mesh)),
                "texture" => new Parameter(DecodeTexture(args), typeof(Texture)),
                "shader" => new Parameter(DecodeShader(args), typeof(Shader)),
                "vec3" => new Parameter(DecodeVector3(v), typeof(Vector3)),
                _ => new Parameter("", typeof(string)) // fallback for unknown
            };
        }

        Debug.LogError($"Unknown parameter format: {v}");
        return new Parameter("", typeof(string));
    }
}
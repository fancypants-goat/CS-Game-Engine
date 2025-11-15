using System.Drawing;
using System.Text.RegularExpressions;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Engine;

internal static class MeshLoader
{
    public static Mesh LoadMesh(string filename, out Material[] materials)
    {
        materials = [];
        
        var extension = Path.GetExtension(filename);
        if (extension == ".obj")
            return ReadObjIntoMesh(filename, out materials);

        
        Debug.LogError("File extension is not supported yet:", extension);
        return new Mesh();
    }

    public static Mesh ReadObjIntoMesh(string filename, out Material[] materials)
    {
        List<Vector3> vertexPositions = [];
        List<Vector3> vertexNormals = [];
        List<Vector2> textureCoordinates = [];

        List<Vertex> resultVertices = [];
        List<int> resultIndices = [];
        List<Material> resultMaterials = [];
        List<Submesh> resultSubmeshes = [];

        int startIndex = 0;

        Dictionary<string, Material> mats = [];

        Material usedMaterial = new();
        bool usingMaterial = false;

        var source = File.ReadAllText(filename);

        foreach (var line in source.Split('\n'))
        {
            var l = line.Trim();

            if (l.StartsWith('#') || string.IsNullOrWhiteSpace(l)) continue;

            const string pattern = @"^(?<command>\w+) \s+ (?<rest>.*)";
            var match = Regex.Match(l, pattern, RegexOptions.IgnorePatternWhitespace);
            if (!match.Success)
            {
                Debug.Log("Failed to decode line");
                continue;
            }
            
            var command = match.Groups["command"].Value;

            switch (command)
            {
                // material commands
                case "mtllib":
                    var name = match.Groups["rest"].Value;
                    var path = Path.Combine(Path.GetDirectoryName(filename), name);
                    mats = ReadMTLIntoMaterials(path);
                    break;
                case "usemtl":
                    if (usingMaterial)
                    {
                        resultMaterials.Add(usedMaterial);
                        resultSubmeshes.Add(new Submesh(startIndex, (int)(resultIndices.Count - startIndex)));
                        
                        startIndex = resultIndices.Count;
                    }
                        
                    usingMaterial = true;
                    mats.TryGetValue(match.Groups["rest"].Value, out usedMaterial);
                    break;
                
                // mesh commands
                case "v":
                    string[] nums1 = match.Groups["rest"].Value.Split(' ');
                    vertexPositions.Add(new Vector3(float.Parse(nums1[0]), float.Parse(nums1[1]), float.Parse(nums1[2])));
                    break;
                case "vn":
                    string[] nums2 = match.Groups["rest"].Value.Split(' ');
                    vertexNormals.Add(new Vector3(float.Parse(nums2[0]), float.Parse(nums2[1]), float.Parse(nums2[2])));
                    break;
                case "vt":
                    string[] nums3 = match.Groups["rest"].Value.Split(' ');
                    textureCoordinates.Add(new Vector2(float.Parse(nums3[0]), float.Parse(nums3[1])));
                    break;
                
                // face command
                case "f":
                    var matches = Regex.Matches(match.Groups["rest"].Value, @"[(?<v>\d+)[/(?<vt>\d*)]?[/(?<vn>\d*)]?]{3,}",
                        RegexOptions.IgnorePatternWhitespace);

                    var facesSTR = matches.Select(m => new string[] {
                        m.Groups["v"].Value,
                        m.Groups["vt"].Value,
                        m.Groups["vn"].Value
                    }).ToArray();

                    List<Vertex> faces = [];
                    
                    foreach (var face in facesSTR)
                    {
                        var v = int.Parse(face[0]);
                        Vector3 vertexNormal;
                        Vector2 textureCoord;
                        textureCoord = int.TryParse(face[1], out var vt)
                            ? textureCoordinates[vt - 1]
                            : new Vector2(0);
                        vertexNormal = int.TryParse(face[2], out var vn)
                            ? vertexNormals[vn - 1]
                            : new Vector3(0);

                        var Vertex = new Vertex {
                            Position = vertexPositions[v - 1],
                            TextureCoordinate = textureCoord,
                            Normal = vertexNormal,
                        };
                        faces.Add(Vertex);
                    }
                    
                    // generate a list of indices (split the shape up into triangles)
                    // this can be done by using the first (v0), last used (vL) and current (vC) index
                    int vL = 1;
                    for (int vC = 2; vC < faces.Count; vC++)
                    {
                        resultIndices.Add(resultVertices.Count);
                        resultIndices.Add(resultVertices.Count + vL);
                        resultIndices.Add(resultIndices.Count + vC);
                    }
                    
                    resultVertices.AddRange(faces);
                    break;
                    
            }
        }

        if (usingMaterial)
        {
            resultMaterials.Add(usedMaterial);
            resultSubmeshes.Add(new Submesh(startIndex, (int)(resultIndices.Count - startIndex)));
        }
        
        materials = resultMaterials.ToArray();
        return new Mesh(resultVertices.ToArray(), resultSubmeshes.ToArray());
    }

    public static Dictionary<string, Material> ReadMTLIntoMaterials(string path)
    {
        Dictionary<string, Material> materials = [];

        string currentMTL = "";
        Material material = new Material();

        string source = File.ReadAllText(path);

        foreach (string line in source.Split('\n'))
        {
            var l = line.Trim();

            if (l.StartsWith('#') || string.IsNullOrWhiteSpace(l)) continue;

            var match = Regex.Match(l, @"\A(?<command>\w+) \s+ (?<rest>.*)", RegexOptions.IgnorePatternWhitespace);
            
            var command = match.Groups["command"].Value;

            switch (command)
            {
                case "newmtl":
                    if (!string.IsNullOrWhiteSpace(currentMTL))
                        materials[currentMTL] = material;
                    
                    currentMTL = match.Groups["rest"].Value;
                    material = new Material();
                    break;
                
                // light / color
                case "Ns":
                    var value = float.Parse(match.Groups["rest"].Value);
                    material.SpecularExponent = value;
                    break;
                case "Ka":
                    float[] valuesA = match.Groups["rest"].Value.Split(' ').Select(float.Parse).ToArray();
                    material.Color = Color.FromArgb(255, (int)(valuesA[0] * 255), (int)(valuesA[1] * 255), (int)(valuesA[2] * 255));
                    break;
                case "Kd":
                    float[] valuesD = match.Groups["rest"].Value.Split(' ').Select(float.Parse).ToArray();
                    material.DiffuseColor = Color.FromArgb(255, (int)(valuesD[0] * 255), (int)(valuesD[1] * 255), (int)(valuesD[2] * 255));
                    break;
                case "Ks":
                    float[] valuesS = match.Groups["rest"].Value.Split(' ').Select(float.Parse).ToArray();
                    material.DiffuseColor = Color.FromArgb(255, (int)(valuesS[0] * 255), (int)(valuesS[1] * 255), (int)(valuesS[2] * 255));
                    break;
                default:
                    Debug.LogWarn("Unsupported mtl attribute:", command);
                    break;
            }
        }
        
        if (!string.IsNullOrWhiteSpace(currentMTL))
            materials[currentMTL] = material;
        
        return materials;
    }
}
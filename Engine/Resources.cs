namespace Engine;

public static class Resources
{
    public static string GetPath(params string[] relativePath)
    {
        var path = Path.Combine(relativePath);
        
        if (Path.Exists(path)) return path;
        
        return Path.Combine(Directory.GetCurrentDirectory(), "Resources", path);
    }


    public static Scene GetScene(string path)
    {
        var data = GetSceneData(path);
        return Scene.CreateFromData(data);
    }

    public static SceneData GetSceneData(string path)
    {
        return SceneLoader.LoadSceneData(GetPath(path));
    }


    public static Mesh? GetMesh(string path)
    {
        path = GetPath(path); // make sure 'path' is a global path

        if (!File.Exists(path))
        {
            Debug.LogError("Path to Object file does not exist:", path, "\n\tPath must be a valid path to a .obj or binary file.");
            return null;
        }

        throw new NotImplementedException("Mesh Loading from File is not yet implemented.");
    }


    public static Shader GetShader(string name)
    {
        var vertPath = GetPath("Shaders", name + ".vert");
        var fragPath = GetPath("Shaders", name + ".frag");
        
        return new Shader(vertPath, fragPath);
    }
}
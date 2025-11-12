namespace Engine;

public static class SceneManager
{
    public static Scene ActiveScene { get; private set; }
    public static Camera MainCamera { get; private set; }


    public static void SetMainCamera(Camera camera)
    {
        MainCamera = camera;
    }

    public static void ActivateScene(Scene? scene)
    {
        // TODO implement scene loading logic
        ActiveScene = scene;
        throw new NotImplementedException("Switching Scenes is not supported yet!");
    }
}
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
        ActiveScene = scene;
        scene?.Reset();
    }
}
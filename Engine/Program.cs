using System.ComponentModel;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine;

public class Window : GameWindow
{
    // TODO create ECS
    // TODO create camera component
    private Shader shader;
    private Texture texture;
    private Entity cube;
    private Entity cameraEntity;
    private Camera camera;

    public Window(int width, int height, string title)
        : base(GameWindowSettings.Default, new NativeWindowSettings
        {
            ClientSize = (width, height),
            Title = title
        })
    { }


    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.4f, 0.2f, 0.5f, 1.0f);

        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Less);

        shader = new Shader(Resources.GetPath("Shaders/shader.vert"), Resources.GetPath("Shaders/shader.frag"));
        shader.Use();
        shader.Uniform3f("color", 1,1,1);

        texture = new Texture(Resources.GetPath("Textures/wall.jpg"), true);

        cube = new Entity();
        // cube.transform.Position = new Vector3(-.5f, .5f, 0);
        var r = new Renderer(cube, shader, texture);
        cube.AddComponent(r);

        cameraEntity = new Entity();
        camera = new Camera(cameraEntity, Camera.CameraType.Perspective, 0.1f, 100f, 90f);
        cameraEntity.AddComponent(camera);
        cameraEntity.Transform.Position = new Vector3(0, 0, -5);
        camera.SetViewportSize(ClientSize.X, ClientSize.Y);
        camera.Update();
    }


    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Debug.LogPrefixed(Debug.LogType.Exit, "Exiting Due to Escape Press");
            Close();
        }

        float speed = 3;
        if (KeyboardState.IsKeyDown(Keys.W))
            camera.Transform.Translate(speed * (float)args.Time, camera.Transform.Forwards);
        if (KeyboardState.IsKeyDown(Keys.S))
            camera.Transform.Translate(speed * (float)args.Time, -camera.Transform.Forwards);
        if (KeyboardState.IsKeyDown(Keys.D))
            camera.Transform.Translate(speed * (float)args.Time, -camera.Transform.Right);
        if (KeyboardState.IsKeyDown(Keys.A))
            camera.Transform.Translate(speed * (float)args.Time, camera.Transform.Right);
        if (KeyboardState.IsKeyDown(Keys.Space))
            camera.Transform.Translate(speed * (float)args.Time, camera.Transform.Up);
        if (KeyboardState.IsKeyDown(Keys.LeftShift))
            camera.Transform.Translate(speed * (float)args.Time, -camera.Transform.Up);

        camera.Update();
        

        var x = (float)(args.Time * 300);
        var y = (float)(Math.Sqrt(args.Time) * 2f);
        var z = (float)(Math.Sin(args.Time)*1);
        cube.Transform.Rotate(x, y, z);
    }


    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        cube.GetComponent<Renderer>()?.Draw(camera);

        SwapBuffers();
    }


    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
        
        camera.SetViewportSize(e.Width, e.Height);
    }


    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        Debug.Log("Disposing Objects");

        cube.Dispose();
        
        Debug.Log("Finished Disposing Objects");
    }
}

internal static class Launch
{
    private static void Main(string[] args)
    {
        Debug.LogPrefixed(Debug.LogType.Launch, "Launching Program, Creating Window");
        
        using Window window = new(600, 600, "OpenTK Game Engine");
        window.Run();
        
        Debug.LogPrefixed(Debug.LogType.Exit, "Closed Window, Exiting Program");
    }
}
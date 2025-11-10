namespace Engine;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

class Window : GameWindow
{
    private float[] vertices = {
        -0.5f, -0.5f, 0.0f, //Bottom-left vertex
        0.5f, -0.5f, 0.0f, //Bottom-right vertex
        0.0f,  0.5f, 0.0f  //Top vertex
    };

    private int vbo;
    
    public Window(int width, int height, string title)
        : base(GameWindowSettings.Default, new NativeWindowSettings()
        {
            ClientSize = (width, height),
            Title = title
        })
    {
    }


    protected override void OnLoad()
    {
        Debug.LogAs(Debug.LogType.Info, "OnLoad()", "called.");
        
        base.OnLoad();

        GL.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
        
        vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsage.StaticDraw);
    }


    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        
        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }
    }


    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        
        GL.Clear(ClearBufferMask.ColorBufferBit);

        SwapBuffers();
    }


    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }
}

class Launch
{
    static void Main(string[] args)
    {
        using (Window window = new(800, 600, "OpenTK Game Engine"))
            window.Run();
    }
}
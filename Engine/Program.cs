using System.ComponentModel;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine;

internal class Window : GameWindow
{
    // TODO create ECS
    // TODO create camera component
    private Shader shader;
    private Texture texture;
    private VertexArrayObject vao;
    
    private VertexBufferObject vbo;
    private ElementBufferObject ebo;

    private float[] vertices = {
        //Position          Texture coordinates
        0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
        0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
        -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
    };
    
    private readonly uint[] indices = {  // note that we start from 0!
        0, 1, 3,   // first triangle
        1, 2, 3    // second triangle
    };
    
    float[] texCoords = {
        0.0f, 0.0f,  // lower-left corner  
        1.0f, 0.0f,  // lower-right corner
        0.5f, 1.0f   // top-center corner
    };

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

        vao = new VertexArrayObject();
        vao.Use();

        vbo = new VertexBufferObject();
        vbo.Upload(vertices, BufferUsage.StaticDraw);
        
        ebo = new ElementBufferObject();
        ebo.Upload(indices, BufferUsage.StaticDraw);

        vao.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 5, 0);
        vao.VertexAttribPointer(1, 2,  VertexAttribPointerType.Float, false, sizeof(float) * 5, sizeof(float) * 3);

        shader = new Shader(Resources.GetPath("Shaders/shader.vert"), Resources.GetPath("Shaders/shader.frag"));
        shader.Use();
        shader.Uniform3f("color", 1,1,1);

        texture = new Texture(Resources.GetPath("Textures/wall.jpg"), true);
    }


    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (KeyboardState.IsKeyDown(Keys.Escape)) Close();
    }


    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        vao.Use();
        shader.Use();
        texture.Use();
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);

        SwapBuffers();
    }


    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }


    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        Debug.Log("Disposing Objects");
        
        vao.Dispose();
        vbo.Dispose();
        ebo.Dispose();
        shader.Dispose();
        texture.Dispose();
        
        Debug.Log("Finished Disposing Objects");
    }
}

internal class Launch
{
    private static void Main(string[] args)
    {
        Debug.LogPrefixed(Debug.LogType.Launch, "Launching Program, Creating Window");
        
        using Window window = new(800, 600, "OpenTK Game Engine");
        window.Run();
        
        Debug.LogPrefixed(Debug.LogType.Exit, "Closed Window, Exiting Program");
    }
}
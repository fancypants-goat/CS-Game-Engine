using OpenTK.Graphics.OpenGL;

namespace Engine;

public class Renderer : Component, IDrawable
{
    private float[] cubeVertices = [
        // Front face (z = +0.5)
        -0.5f, -0.5f,  0.5f,   0.0f, 0.0f,   0.0f,  0.0f,  1.0f, // Bottom-left
         0.5f, -0.5f,  0.5f,   1.0f, 0.0f,   0.0f,  0.0f,  1.0f, // Bottom-right
         0.5f,  0.5f,  0.5f,   1.0f, 1.0f,   0.0f,  0.0f,  1.0f, // Top-right
        -0.5f,  0.5f,  0.5f,   0.0f, 1.0f,   0.0f,  0.0f,  1.0f, // Top-left

        // Back face (z = -0.5)
         0.5f, -0.5f, -0.5f,   0.0f, 0.0f,   0.0f,  0.0f, -1.0f, // Bottom-left
        -0.5f, -0.5f, -0.5f,   1.0f, 0.0f,   0.0f,  0.0f, -1.0f, // Bottom-right
        -0.5f,  0.5f, -0.5f,   1.0f, 1.0f,   0.0f,  0.0f, -1.0f, // Top-right
         0.5f,  0.5f, -0.5f,   0.0f, 1.0f,   0.0f,  0.0f, -1.0f, // Top-left

        // Left face (x = -0.5)
        -0.5f, -0.5f, -0.5f,   0.0f, 0.0f,  -1.0f,  0.0f,  0.0f, // Bottom-left
        -0.5f, -0.5f,  0.5f,   1.0f, 0.0f,  -1.0f,  0.0f,  0.0f, // Bottom-right
        -0.5f,  0.5f,  0.5f,   1.0f, 1.0f,  -1.0f,  0.0f,  0.0f, // Top-right
        -0.5f,  0.5f, -0.5f,   0.0f, 1.0f,  -1.0f,  0.0f,  0.0f, // Top-left

        // Right face (x = +0.5)
         0.5f, -0.5f,  0.5f,   0.0f, 0.0f,   1.0f,  0.0f,  0.0f, // Bottom-left
         0.5f, -0.5f, -0.5f,   1.0f, 0.0f,   1.0f,  0.0f,  0.0f, // Bottom-right
         0.5f,  0.5f, -0.5f,   1.0f, 1.0f,   1.0f,  0.0f,  0.0f, // Top-right
         0.5f,  0.5f,  0.5f,   0.0f, 1.0f,   1.0f,  0.0f,  0.0f, // Top-left

        // Bottom face (y = -0.5)
        -0.5f, -0.5f, -0.5f,   0.0f, 0.0f,   0.0f, -1.0f,  0.0f, // Back-left
         0.5f, -0.5f, -0.5f,   1.0f, 0.0f,   0.0f, -1.0f,  0.0f, // Back-right
         0.5f, -0.5f,  0.5f,   1.0f, 1.0f,   0.0f, -1.0f,  0.0f, // Front-right
        -0.5f, -0.5f,  0.5f,   0.0f, 1.0f,   0.0f, -1.0f,  0.0f, // Front-left

        // Top face (y = +0.5)
        -0.5f,  0.5f,  0.5f,   0.0f, 0.0f,   0.0f,  1.0f,  0.0f, // Front-left
         0.5f,  0.5f,  0.5f,   1.0f, 0.0f,   0.0f,  1.0f,  0.0f, // Front-right
         0.5f,  0.5f, -0.5f,   1.0f, 1.0f,   0.0f,  1.0f,  0.0f, // Back-right
        -0.5f,  0.5f, -0.5f,   0.0f, 1.0f,   0.0f,  1.0f,  0.0f  // Back-left
    ];


    private uint[] cubeIndices =
    [
     0, 1, 2, 2, 3, 0, // Front
     4, 5, 6, 6, 7, 4, // Back
     8, 9, 10, 10, 11, 8, // Left
     12, 13, 14, 14, 15, 12, // Right
     16, 17, 18, 18, 19, 16, // Bottom
     20, 21, 22, 22, 23, 20 // Top
    ];

    
    protected VertexArrayObject vao;
    protected VertexBufferObject vbo;
    protected ElementBufferObject? ebo;

    public Shader shader;
    public Texture? texture;

    public Renderer(Entity parent) : base(parent)
    {
        vao = new VertexArrayObject();
        vbo = new VertexBufferObject();
        vbo.Upload(cubeVertices, BufferUsage.StaticDraw);
        ebo = new ElementBufferObject();
        ebo.Upload(cubeIndices, BufferUsage.StaticDraw);
        
        shader = new Shader(Resources.GetPath("Shaders/shader.vert"), Resources.GetPath("Shaders/shader.frag"));
        texture = null;
    }

    public void Draw()
    {
        throw new NotImplementedException();
    }
}
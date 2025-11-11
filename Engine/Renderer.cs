using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.Vulkan;

namespace Engine;

public class Renderer : Component, IDrawable
{
	private static float[] cubeVertices = [
		// Front face
		-0.5f, -0.5f,  0.5f,   0.0f, 0.0f,   0.0f,  0.0f,  1.0f, // Bottom-left
		 0.5f, -0.5f,  0.5f,   1.0f, 0.0f,   0.0f,  0.0f,  1.0f, // Bottom-right
		 0.5f,  0.5f,  0.5f,   1.0f, 1.0f,   0.0f,  0.0f,  1.0f, // Top-right
		-0.5f,  0.5f,  0.5f,   0.0f, 1.0f,   0.0f,  0.0f,  1.0f, // Top-left
		
		// Back face
		 0.5f, -0.5f, -0.5f,   0.0f, 0.0f,   0.0f,  0.0f, -1.0f, // Bottom-left
		-0.5f, -0.5f, -0.5f,   1.0f, 0.0f,   0.0f,  0.0f, -1.0f, // Bottom-right
		-0.5f,  0.5f, -0.5f,   1.0f, 1.0f,   0.0f,  0.0f, -1.0f, // Top-right
		 0.5f,  0.5f, -0.5f,   0.0f, 1.0f,   0.0f,  0.0f, -1.0f, // Top-left
		
		// Left face
		-0.5f, -0.5f, -0.5f,   0.0f, 0.0f,  -1.0f,  0.0f,  0.0f, // Bottom-left
		-0.5f, -0.5f,  0.5f,   1.0f, 0.0f,  -1.0f,  0.0f,  0.0f, // Bottom-right
		-0.5f,  0.5f,  0.5f,   1.0f, 1.0f,  -1.0f,  0.0f,  0.0f, // Top-right
		-0.5f,  0.5f, -0.5f,   0.0f, 1.0f,  -1.0f,  0.0f,  0.0f, // Top-left
		
		// Right face
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


    private static uint[] cubeIndices = [
		0, 1, 2, 2, 3, 0, // Front
		4, 5, 6, 6, 7, 4, // Back
		8, 9, 10, 10, 11, 8, // Left
		12, 13, 14, 14, 15, 12, // Right
		16, 17, 18, 18, 19, 16, // Bottom
		20, 21, 22, 22, 23, 20 // Top
    ];

    
    protected VertexArrayObject Vao;
    protected VertexBufferObject Vbo;
    protected ElementBufferObject? Ebo;

    public Shader Shader;
    public Texture? Texture;

    public Renderer(Entity parent) : base(parent)
    {
        Shader = new Shader(Resources.GetPath("Shaders/shader.vert"), Resources.GetPath("Shaders/shader.frag"));
        Texture = null;
        
        Initialize();
    }

    public Renderer(Entity parent, Shader? shader, Texture? texture) : base(parent)
    {
	    shader ??= new Shader(Resources.GetPath("Shaders/shader.vert"), Resources.GetPath("Shaders/shader.frag"));
	    Shader = shader;
	    Texture = texture;

	    Initialize();
    }

    protected void Initialize()
    {
	    Vao = new VertexArrayObject();
	    Vao.Use();
	    Vbo = new VertexBufferObject();
	    Vbo.Upload(cubeVertices, BufferUsage.StaticDraw);
	    Ebo = new ElementBufferObject();
	    Ebo.Upload(cubeIndices, BufferUsage.StaticDraw);

	    const int stride = 8 * sizeof(float);
	    Vao.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
	    Vao.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
	    Vao.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, 5 * sizeof(float));
    }

    public void Draw(Camera camera)
    {
	    Vao.Use();
	    Shader.Use();
	    Shader.UniformMat4("model", false, Transform.ModelMatrix);
	    Shader.UniformMat4("camera", false, camera.View * camera.Projection);
	    Texture?.Use();
	    
	    if (Ebo != null)
		    GL.DrawElements(PrimitiveType.Triangles, cubeIndices.Length, DrawElementsType.UnsignedInt, 0);
	    else
		    GL.DrawArrays(PrimitiveType.Triangles, 0, cubeVertices.Length / 8);
    }


    
    protected override void Dispose(bool disposing)
    {
	    if (_isDisposed) return;

	    if (disposing)
	    {
		    GL.BindVertexArray(0);
		    Vao.Dispose();
		    Vbo.Dispose();
		    Ebo?.Dispose();
		    Shader.Dispose();
		    Texture?.Dispose();
	    }
	    
	    _isDisposed = true;
    }
    
    ~Renderer()
    {
	    if (_isDisposed) return;
	    
	    Debug.LogMemLeak("Renderer");
	    Dispose(false);
    }
}
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.Vulkan;
using OpenTK.Mathematics;

namespace Engine;

[ComponentMeta("Renderer")]
public class Renderer : Component, IDrawable
{
	// TODO Add a MaterialPropertyBlock system (like Unity) so multiple renderers can share a material but override a few properties (e.g., color).
	// TODO Add a RendererType distinction later (MeshRenderer, SkinnedMeshRenderer, SpriteRenderer, etc.).
	// TODO Add batching logic later based on shared Mesh and Material.
	
	private static readonly Vertex[] cubeVertices =
	{
	    // ─────────────────────────────
	    // FRONT
	    // ─────────────────────────────
	    new Vertex { Position = new Vector3(-0.5f, -0.5f,  0.5f), TextureCoordinate = new Vector2(0.0f, 0.0f), Normal = new Vector3(0f, 0f, 1f) },
	    new Vertex { Position = new Vector3( 0.5f, -0.5f,  0.5f), TextureCoordinate = new Vector2(1.0f, 0.0f), Normal = new Vector3(0f, 0f, 1f) },
	    new Vertex { Position = new Vector3( 0.5f,  0.5f,  0.5f), TextureCoordinate = new Vector2(1.0f, 1.0f), Normal = new Vector3(0f, 0f, 1f) },
	    new Vertex { Position = new Vector3(-0.5f,  0.5f,  0.5f), TextureCoordinate = new Vector2(0.0f, 1.0f), Normal = new Vector3(0f, 0f, 1f) },

	    // ─────────────────────────────
	    // BACK
	    // ─────────────────────────────
	    new Vertex { Position = new Vector3( 0.5f, -0.5f, -0.5f), TextureCoordinate = new Vector2(0.0f, 0.0f), Normal = new Vector3(0f, 0f, -1f) },
	    new Vertex { Position = new Vector3(-0.5f, -0.5f, -0.5f), TextureCoordinate = new Vector2(1.0f, 0.0f), Normal = new Vector3(0f, 0f, -1f) },
	    new Vertex { Position = new Vector3(-0.5f,  0.5f, -0.5f), TextureCoordinate = new Vector2(1.0f, 1.0f), Normal = new Vector3(0f, 0f, -1f) },
	    new Vertex { Position = new Vector3( 0.5f,  0.5f, -0.5f), TextureCoordinate = new Vector2(0.0f, 1.0f), Normal = new Vector3(0f, 0f, -1f) },

	    // ─────────────────────────────
	    // LEFT
	    // ─────────────────────────────
	    new Vertex { Position = new Vector3(-0.5f, -0.5f, -0.5f), TextureCoordinate = new Vector2(0.0f, 0.0f), Normal = new Vector3(-1f, 0f, 0f) },
	    new Vertex { Position = new Vector3(-0.5f, -0.5f,  0.5f), TextureCoordinate = new Vector2(1.0f, 0.0f), Normal = new Vector3(-1f, 0f, 0f) },
	    new Vertex { Position = new Vector3(-0.5f,  0.5f,  0.5f), TextureCoordinate = new Vector2(1.0f, 1.0f), Normal = new Vector3(-1f, 0f, 0f) },
	    new Vertex { Position = new Vector3(-0.5f,  0.5f, -0.5f), TextureCoordinate = new Vector2(0.0f, 1.0f), Normal = new Vector3(-1f, 0f, 0f) },

	    // ─────────────────────────────
	    // RIGHT
	    // ─────────────────────────────
	    new Vertex { Position = new Vector3( 0.5f, -0.5f,  0.5f), TextureCoordinate = new Vector2(0.0f, 0.0f), Normal = new Vector3(1f, 0f, 0f) },
	    new Vertex { Position = new Vector3( 0.5f, -0.5f, -0.5f), TextureCoordinate = new Vector2(1.0f, 0.0f), Normal = new Vector3(1f, 0f, 0f) },
	    new Vertex { Position = new Vector3( 0.5f,  0.5f, -0.5f), TextureCoordinate = new Vector2(1.0f, 1.0f), Normal = new Vector3(1f, 0f, 0f) },
	    new Vertex { Position = new Vector3( 0.5f,  0.5f,  0.5f), TextureCoordinate = new Vector2(0.0f, 1.0f), Normal = new Vector3(1f, 0f, 0f) },

	    // ─────────────────────────────
	    // BOTTOM
	    // ─────────────────────────────
	    new Vertex { Position = new Vector3(-0.5f, -0.5f, -0.5f), TextureCoordinate = new Vector2(0.0f, 0.0f), Normal = new Vector3(0f, -1f, 0f) },
	    new Vertex { Position = new Vector3( 0.5f, -0.5f, -0.5f), TextureCoordinate = new Vector2(1.0f, 0.0f), Normal = new Vector3(0f, -1f, 0f) },
	    new Vertex { Position = new Vector3( 0.5f, -0.5f,  0.5f), TextureCoordinate = new Vector2(1.0f, 1.0f), Normal = new Vector3(0f, -1f, 0f) },
	    new Vertex { Position = new Vector3(-0.5f, -0.5f,  0.5f), TextureCoordinate = new Vector2(0.0f, 1.0f), Normal = new Vector3(0f, -1f, 0f) },

	    // ─────────────────────────────
	    // TOP
	    // ─────────────────────────────
	    new Vertex { Position = new Vector3(-0.5f,  0.5f,  0.5f), TextureCoordinate = new Vector2(0.0f, 0.0f), Normal = new Vector3(0f, 1f, 0f) },
	    new Vertex { Position = new Vector3( 0.5f,  0.5f,  0.5f), TextureCoordinate = new Vector2(1.0f, 0.0f), Normal = new Vector3(0f, 1f, 0f) },
	    new Vertex { Position = new Vector3( 0.5f,  0.5f, -0.5f), TextureCoordinate = new Vector2(1.0f, 1.0f), Normal = new Vector3(0f, 1f, 0f) },
	    new Vertex { Position = new Vector3(-0.5f,  0.5f, -0.5f), TextureCoordinate = new Vector2(0.0f, 1.0f), Normal = new Vector3(0f, 1f, 0f) }
	};



    private static uint[] cubeIndices = [
		0, 1, 2, 2, 3, 0, // Front
		4, 5, 6, 6, 7, 4, // Back
		8, 9, 10, 10, 11, 8, // Left
		12, 13, 14, 14, 15, 12, // Right
		16, 17, 18, 18, 19, 16, // Bottom
		20, 21, 22, 22, 23, 20 // Top
    ];
    
    
    public Material[] Materials { get; set; }
    public Mesh Mesh { get; set; }
    

    public Renderer(Entity parent) : base(parent) // default constructor
    {
	    Materials = [
		    new Material
		    {
			    Shader = Resources.GetShader("default"),
			    Texture = null,
			    Color = Color.White
		    }
	    ];
	    
	    Mesh = new Mesh(cubeVertices, cubeIndices);
	    var stride = 8 * sizeof(float);
	    Mesh.VertexArrayObject.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
	    Mesh.VertexArrayObject.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
	    Mesh.VertexArrayObject.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, 5 * sizeof(float));
    }

    public Renderer(Entity parent, Shader? shader, Texture? texture, Color? color) : base(parent)
    {
	    shader ??= Resources.GetShader("default");
	    color ??= Color.White;
	    
	    Materials = [
		    new Material
		    {
			    Shader = shader,
			    Texture = texture,
			    Color = (Color)color
		    }
	    ];
	    
	    Mesh = new Mesh(cubeVertices, cubeIndices);
	    var stride = 8 * sizeof(float);
	    Mesh.VertexArrayObject.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
	    Mesh.VertexArrayObject.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
	    Mesh.VertexArrayObject.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, 5 * sizeof(float));
    }

    public Renderer(Entity parent, Mesh mesh, Material[] materials) : base(parent)
    {
	    Materials = materials;
	    Mesh = mesh;
    }

    public Renderer(Entity parent, Mesh mesh, Material[] materials, Shader shader, Texture texture) : base(parent)
    {
	    Materials = materials;
	    for (int i = 0; i < materials.Length; i++)
	    {
		    materials[i].Shader = shader;
		    materials[i].Texture = texture;
	    }
	    Mesh = mesh;
    }

    public Renderer(Entity parent, Mesh mesh, Material[] materials, Shader shader) : base(parent)
    {
	    Materials = materials;
	    for (int i = 0; i < materials.Length; i++)
	    {
		    materials[i].Shader = shader;
	    }
	    Mesh = mesh;
    }
	
    public void Draw(Camera camera)
    {
	    if (Mesh.Submeshes.Length == 0)
	    {
		    GL.DrawArrays(PrimitiveType.Triangles, 0, Mesh.Submeshes.Length);
		    return;
	    }
	    
	    for (int i = 0; i < Mesh.Submeshes.Length; i++)
	    {
		    var mat = Materials[i];
		    mat.Use();
		    mat.Shader.UniformMat4("model", false, Transform.ModelMatrix);
		    mat.Shader.UniformMat4("camera", false, camera.View * camera.Projection);
		    mat.Shader.Uniform3f("color", mat.Color.R / 255, mat.Color.G / 255, mat.Color.B / 255);
		    mat.Shader.Uniform1i("texture0", 0);
		    Mesh.DrawSubmesh(i);
	    }
    }


    protected override void Dispose(bool disposing)
    {
	    if (IsDisposed) return;
	    
	    if (disposing)
	    {
		    for (int i = 0; i < Materials.Length; i++)
			    Materials[i].Dispose();

		    Mesh.Dispose();
	    }
	    
	    IsDisposed = true;
    }

    ~Renderer()
    {
	    if (IsDisposed) return;
	    
	    Debug.LogMemLeak("Renderer");
	    Dispose(false);
    }
}
using OpenTK.Graphics.OpenGL;

namespace Engine;

/// <summary>
/// Mesh contains the vertex data of a mesh<br/>
/// this includes:<br/>
/// (basics) VertexArrayObject, VertexBufferObject, Vertices<br/>
/// (optional) Submesh[]<br/>
/// The Submesh[] contains the index data of the submeshes and each Submesh is usually paired with a Material in any IDrawable
/// </summary>
public struct Mesh : IDisposable
{
    private Vertex[] _vertices;
    public Vertex[] Vertices
    {
        get
        {
            return _vertices;
        }
        set
        {
            _vertices = value;
            VertexBufferObject.Upload(_vertices, BufferUsage.StaticDraw);
        }
    }
    private uint[] _indices;
    public uint[] Indices
    {
        get
        {
            return _indices;
        }
        set
        {
            _indices = value;
            ElementBufferObject.Upload(_indices, BufferUsage.StaticDraw);
        }
    }
    
    
    public VertexArrayObject VertexArrayObject { get; private set; } = new();
    public VertexBufferObject VertexBufferObject { get; private set; } = new();
    public ElementBufferObject ElementBufferObject { get; private set; }
    
    public Submesh[] Submeshes { get; }
    
    private bool _isDisposed = false;

    public Mesh(Vertex[] vertices, uint[]? indices)
    {
        _vertices = vertices;

        VertexArrayObject.Use();
        VertexBufferObject.Upload(vertices, BufferUsage.StaticDraw);

        if (indices != null)
        {
            ElementBufferObject = new ElementBufferObject();
            ElementBufferObject.Upload(indices, BufferUsage.StaticDraw);
            Submeshes = [new Submesh(0, indices.Length)];
        }
        else Submeshes = [];
    }
    public Mesh(Vertex[] vertices, Submesh[] submeshes)
    {
        _vertices = vertices;
        
        VertexArrayObject.Use();
        VertexBufferObject.Upload(vertices, BufferUsage.StaticDraw);
        
        Submeshes = submeshes;
    }

    public void Use()
    {
        VertexBufferObject.Use();
    }

    public void DrawSubmesh(int index)
    {
        var sub = Submeshes[index];
        Use();
        GL.DrawElements(PrimitiveType.Triangles, sub.IndexCount, DrawElementsType.UnsignedInt, sub.IndexStart * sizeof(uint));
    }
    
    public void Dispose()
    {
        if (_isDisposed) return;

        ElementBufferObject?.Dispose();
        VertexArrayObject.Dispose();
        VertexBufferObject.Dispose();
        
        _isDisposed = true;
    }
}
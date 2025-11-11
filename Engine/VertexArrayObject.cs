using OpenTK.Graphics.OpenGL;

namespace Engine;

public class VertexArrayObject : IDisposable
{
    public readonly int Handle;

    private bool _isDisposed;

    public VertexArrayObject()
    {
        Handle = GL.GenVertexArray();
    }

    public void Use()
    {
        GL.BindVertexArray(Handle);
    }


    public void VertexAttribPointer(uint location, int size, VertexAttribPointerType type, bool normalized, int stride,
        int offset)
    {
        Use();
        GL.VertexAttribPointer(location, size, type, normalized, stride, offset);
        GL.EnableVertexAttribArray(location);
    }


    private void Dispose(bool dispose)
    {
        if (_isDisposed) return;

        GL.BindVertexArray(0);
        GL.DeleteVertexArray(Handle);

        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~VertexArrayObject()
    {
        if (_isDisposed) return;

        Debug.LogWarn("Memory leak detected in VertexArrayObject instance! Did not call Dispose().");
        Dispose(false);
    }
}
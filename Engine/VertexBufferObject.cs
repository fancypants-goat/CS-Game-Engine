using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;

namespace Engine;

public class VertexBufferObject : IDisposable
{
    public readonly int Handle;

    private bool _isDisposed;

    public VertexBufferObject()
    {
        Handle = GL.GenBuffer();
    }

    public void Upload<T>(T[] data, BufferUsage usage) where T : unmanaged
    {
        Use();
        var size = data.Length * Unsafe.SizeOf<T>();
        GL.BufferData(BufferTarget.ArrayBuffer, size, data, usage);
    }

    public void Use()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, Handle);
    }


    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        GL.DeleteBuffer(Handle);

        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~VertexBufferObject()
    {
        if (_isDisposed) return;

        Debug.LogWarn("Memory leak detected in VertexBufferObject instance! Did not call Dispose().");
        Dispose(false);
    }
}
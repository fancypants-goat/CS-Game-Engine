using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;

namespace Engine;

public class ElementBufferObject : IDisposable
{
    public readonly int Handle;

    private bool _isDisposed;

    public ElementBufferObject()
    {
        Handle = GL.GenBuffer();
    }
    
    public void Upload(uint[] data, BufferUsage usage)
    {
        Use();
        GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(uint), data, usage);
    }

    public void Use()
    {
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, Handle);
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

    ~ElementBufferObject()
    {
        Debug.LogWarn("Memory leak detected in ElementBufferObject instance! Did not call Dispose().");
        Dispose(false);
    }
}
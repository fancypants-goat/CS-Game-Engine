namespace Engine;

public class Component : IDisposable
{
    public Entity parent;
    public Transform transform { get; }

    protected bool _isDisposed;

    public Component(Entity parent)
    {
        this.parent = parent;
        transform = parent.transform;

        parent.AddComponent(this);
    }

    public virtual void Load()
    {
        
    }

    public virtual void Update()
    {
        
    }

    public virtual void Unload()
    {
        parent.RemoveComponent(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            Unload();
        }
        
        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Component()
    {
        if (_isDisposed) return;
        
        Dispose(false);
        Debug.LogWarn("Memory leak detected in Component instance! Did not call Dispose().");
    }
}
namespace Engine;


public class Component : IDisposable
{
    public Entity Parent;
    public Transform Transform { get; }

    protected bool _isDisposed;

    public Component(Entity parent)
    {
        this.Parent = parent;
        Transform = parent.Transform;
    }
    
    

    public virtual void Load()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void Unload()
    {
        Parent.RemoveComponent(this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Component()
    {
        if (_isDisposed) return;
        
        Debug.LogMemLeak("Component");
        Dispose(false);
    }
}
namespace Engine;

[ComponentMeta("Component")]
public class Component(Entity parent) : IDisposable
{
    public bool Enabled = true;
    
    public Entity Parent = parent;
    public Transform Transform { get; } = parent.Transform;

    protected bool IsDisposed = false;
    
    

    public virtual void Load()
    {
    }

    public virtual void EarlyUpdate()
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
        if (IsDisposed) return;
        
        Debug.LogMemLeak("Component");
        Dispose(false);
    }
}
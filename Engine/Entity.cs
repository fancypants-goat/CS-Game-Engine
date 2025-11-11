using OpenTK.Mathematics;

namespace Engine;

public class Entity : IDisposable
{
    public bool isActive = true;

    public Transform transform;

    protected List<Component> _components;

    protected bool _isDisposed;

    public Entity()
    {
        transform = new Transform(this);
        _components = [transform];
    }


    public virtual void Load()
    {
        foreach (var c in _components)
            c.Load();
    }

    public virtual void Update()
    {
        if (!isActive) return;
        
        foreach (var c in _components)
            c.Update();
    }

    public virtual void Unload()
    {
        foreach (var c in _components)
            c.Unload();
    }



    public void AddComponent(Component component)
    {
        _components.Add(component);
    }

    public T GetComponent<T>(bool includeDisabled = false) where T : Component
    {
        foreach (var component in _components)
        {
            if (component is T c)
                return c;
        }

        return null;
    }

    public void RemoveComponent(Component component)
    {
        _components.Remove(component);
    }


    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            foreach (var c in _components) c.Dispose();
        }
        
        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Entity()
    {
        if (_isDisposed) return;
        
        Dispose(false);
        Debug.LogWarn("Memory leak detected in Entity instance! Did not call Dispose().");
    }
}
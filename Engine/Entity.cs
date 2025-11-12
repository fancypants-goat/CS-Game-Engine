using OpenTK.Mathematics;

namespace Engine;

public class Entity : IDisposable
{
    public bool IsActive = true;

    public Transform Transform;

    protected List<Component> _components;

    protected bool _isDisposed;

    public Entity()
    {
        Transform = new Transform(this);
        _components = [Transform];
    }


    public virtual void Load()
    {
        foreach (var c in _components)
            c.Load();
    }

    public virtual void EarlyUpdate()
    {
        if (!IsActive) return;

        Component? currentComponent = null;
        try
        {
            foreach (var c in _components)
            {
                currentComponent = c;
                c.EarlyUpdate();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error Occured when Updating Component", currentComponent, "|", e.Message, '\n', e);
        }
    }

    public virtual void Update()
    {
        if (!IsActive) return;

        Component? currentComponent = null;
        try
        {
            foreach (var c in _components)
            {
                currentComponent = c;
                c.Update();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error Occured when Updating Component", currentComponent, "|", e.Message, '\n', e);
        }
    }

    public virtual void Unload()
    {
        Component[] unloadComponents = new Component[_components.Count];
        _components.CopyTo(unloadComponents);
        foreach (var c in unloadComponents)
            c.Unload();
    }



    public void AddComponent(Component component)
    {
        _components.Add(component);
    }

    public T? GetComponent<T>(bool includeDisabled = false) where T : Component
    {
        foreach (var component in _components)
        {
            if (component is T c && (includeDisabled || component.Enabled))
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
        
        Debug.LogMemLeak("Entity");
        Dispose(false);
    }
}
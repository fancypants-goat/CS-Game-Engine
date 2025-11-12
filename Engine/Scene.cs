namespace Engine;

public class Scene(string name) : IDisposable
{
    public readonly string Name = name;
    
    private List<Entity> _rootEntities = [];
    private List<IDrawable> _drawables = [];

    private bool _isDisposed;


    public void AddEntity(Entity entity)
    {
        _rootEntities.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        _rootEntities.Remove(entity);
    }

    public Entity GetEntity(int index)
    {
        return _rootEntities[index];
    }

    public void AddDrawable(Entity drawable)
    {
        var renderer = drawable.GetComponent<Renderer>(true);
        
        if (renderer != null)
            _drawables.Add(renderer);
    }

    public void AddDrawable<T>(T drawable) where T : IDrawable
    {
        _drawables.Add(drawable);
    }

    public void RemoveDrawable(Entity drawable)
    {
        var renderer = drawable.GetComponent<Renderer>(true);
        
        if (renderer != null)
            _drawables.Remove(renderer);
    }

    public void RemoveDrawable<T>(T drawable) where T : IDrawable
    {
        _drawables.Remove(drawable);
    }

    public IDrawable GetDrawable(int index)
    {
        return _drawables[index];
    }

    public void Update()
    {
        try
        {
            foreach (var entity in _rootEntities)
                entity.Update();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error Occured when Updating Entity |", e.Message, '\n', e);
        }
    }

    public void Render()
    {
        Debug.Log("Rendering scene", _drawables.Count);
        SceneManager.MainCamera.Render(_drawables);
    }


    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            var entities = new Entity[_rootEntities.Count];
            _rootEntities.CopyTo(entities);
            foreach (var entity in entities)
                entity.Dispose();
        }

        _rootEntities.Clear();
        _drawables.Clear();

        SceneManager.ActivateScene(null);
        
        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Scene()
    {
        if (_isDisposed) return;
        
        Debug.LogMemLeak("Scene");
        Dispose(false);
    }
}
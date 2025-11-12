namespace Engine;

public class Scene(string name) : IDisposable
{
    public readonly string Name = name;
    
    public struct SceneState
    {
        public List<Entity> Entities;
        public List<IDrawable> Drawables;

        public void CopyTo(SceneState state)
        {
            state.Entities.Clear();
            state.Entities.AddRange(Entities);
            
            state.Drawables.Clear();
            state.Drawables.AddRange(Drawables);
        }
    }

    private SceneState _currentState = new SceneState() { Entities = [], Drawables = [] };
    private SceneState _defaultState = new SceneState() { Entities = [], Drawables = [] };


    private bool _isDisposed;


    public void AddEntity(Entity entity)
    {
        _currentState.Entities.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        _currentState.Entities.Remove(entity);
    }

    public Entity GetEntity(int index)
    {
        return _currentState.Entities[index];
    }

    public void AddDrawable(Entity drawable)
    {
        var renderer = drawable.GetComponent<Renderer>(true);
        
        if (renderer != null)
            _currentState.Drawables.Add(renderer);
    }

    public void AddDrawable<T>(T drawable) where T : IDrawable
    {
        _currentState.Drawables.Add(drawable);
    }

    public void RemoveDrawable(Entity drawable)
    {
        var renderer = drawable.GetComponent<Renderer>(true);
        
        if (renderer != null)
            _currentState.Drawables.Remove(renderer);
    }

    public void RemoveDrawable<T>(T drawable) where T : IDrawable
    {
        _currentState.Drawables.Remove(drawable);
    }

    public IDrawable GetDrawable(int index)
    {
        return _currentState.Drawables[index];
    }

    public void Initialize()
    {
        _currentState.CopyTo(_defaultState);
    }

    public void Reset()
    {
        _defaultState.CopyTo(_currentState);
    }

    public void EarlyUpdate()
    {
        try
        {
            foreach (var entity in _currentState.Entities)
                entity.EarlyUpdate();
        }
        catch (Exception e)
        {
            Debug.LogError("Error Occured when EarlyUpdating Entity |", e.Message, '\n', e);
        }
    }
    
    public void Update()
    {
        try
        {
            foreach (var entity in _currentState.Entities)
                entity.Update();
        }
        catch (Exception e)
        {
            Debug.LogError("Error Occured when Updating Entity |", e.Message, '\n', e);
        }
    }

    public void Render()
    {
        SceneManager.MainCamera.Render(_currentState.Drawables);
    }


    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            var entities = new Entity[_currentState.Entities.Count];
            _currentState.Entities.CopyTo(entities);
            foreach (var entity in entities)
                entity.Dispose();
        }

        _currentState.Entities.Clear();
        _currentState.Drawables.Clear();

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
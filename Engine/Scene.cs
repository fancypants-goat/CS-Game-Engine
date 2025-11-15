using OpenTK.Graphics.Vulkan;

namespace Engine;

public class Scene : IDisposable
{
    public readonly string Name;
    private string _path;
    public Camera ActiveCamera { get; set; }
    

    private List<Entity> _entities = [];
    private List<IDrawable> _drawables = [];


    private bool _isDisposed = false;


    /// <summary>
    /// Called when creating a scene from path
    /// </summary>
    /// <param name="data">the data for this scene</param>
    /// <returns></returns>
    public static Scene CreateFromData(SceneData data)
    {
        var scene = new Scene(data.Name);
        scene._path = data.Path;

        scene._entities = data.Entities;
        scene._drawables = data.Drawables;
        
        return scene;
    }

    internal Scene(string name)
    {
        Name = name;
    }


    public void AddEntity(Entity entity)
    {
        _entities.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        _entities.Remove(entity);
    }

    public Entity GetEntity(int index)
    {
        return _entities[index];
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

    
    /// <summary>
    /// Should only be called when creating a scene from code after all initial entities have been added.<br/>
    /// This method writes the scene's current data to a .scene file in the Temp/Scenes folder to allow resetting this scene.
    /// </summary>

    public void Reset()
    {
        var sceneData = Resources.GetSceneData(_path);
        _entities = sceneData.Entities;
        _drawables = sceneData.Drawables;
        ActiveCamera = sceneData.ActiveCamera;
    }

    public void EarlyUpdate()
    {
        try
        {
            foreach (var entity in _entities)
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
            foreach (var entity in _entities)
                entity.Update();
        }
        catch (Exception e)
        {
            Debug.LogError("Error Occured when Updating Entity |", e.Message, '\n', e);
        }
    }

    public void Render()
    {
        ActiveCamera.Render(_drawables);
    }


    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            var entities = new Entity[_entities.Count];
            _entities.CopyTo(entities);
            foreach (var entity in entities)
                entity.Dispose();
        }

        _entities.Clear();
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
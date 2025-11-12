namespace Engine;

public interface IDrawable : IDisposable
{
    public void Draw(Camera camera);
}
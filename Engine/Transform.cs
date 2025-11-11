using OpenTK.Mathematics;

namespace Engine;

public class Transform : Component
{
    public Vector3 position;


    public Transform(Entity parent) : base(parent)
    {
        position = new Vector3(0, 0, 0);
    }

    public Transform(Vector3 position, Entity parent) : base(parent)
    {
        this.position = position;
    }
}
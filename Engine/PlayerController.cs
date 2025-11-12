namespace Engine;

public class PlayerController : Component
{
    public float speed = 2.5f;

    public PlayerController(Entity parent) : base(parent)
    {
    }

    public override void Update()
    {
        base.Update();
        
    }
}
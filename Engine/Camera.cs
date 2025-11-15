using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Engine;

[ComponentMeta("Camera")]
public class Camera : Component
{
    public static Camera Main => SceneManager.ActiveCamera;

    public enum CameraType
    {
        Perspective,
        Orthographic
    }

    public CameraType Type;
    
    // Perspective Camera Variables
    private float _fovy;
    public float Fovy
    {
        get
        {
            return _fovy;
        }
        set
        {
            _fovy = Math.Clamp(value, 1f, 179f);
        }
    }
    
    private int _viewportWidth;
    private int _viewportHeight;
    private float _aspectRatio;
    
    // Orthographic Camera Variables
    public Vector2 Size;
    
    // Global Camera Variables
    public float MaxDepth;
    public float MinDepth;

    public Matrix4 Projection { get; private set; }
    public Matrix4 View { get; private set; }
    
    public Camera(Entity parent) : base(parent)
    {
        Type = CameraType.Perspective;
        Fovy = 90.0f;
        MaxDepth = 100f;
        MinDepth = 1f;
    }

    public Camera(Entity parent, CameraType cameraType, float minDepth, float maxDepth, float fovy = 90.0f, Vector2? size = null)
        : base(parent)
    {
        Type = cameraType;
        MinDepth = minDepth;
        MaxDepth = maxDepth;
        Fovy = fovy;
        Size = size ?? Vector2.One;
    }
    public Camera(Entity parent, int cameraType, float minDepth, float maxDepth, float fovy = 90.0f, Vector2? size = null)
        : base(parent)
    {
        Type = (CameraType)cameraType;
        MinDepth = minDepth;
        MaxDepth = maxDepth;
        Fovy = fovy;
        Size = size ?? Vector2.One;
    }

    public override void Update()
    {
        base.Update();

        
        View = Matrix4.LookAt(Transform.Position, Transform.Position + Transform.Forwards, Vector3.UnitY);
        Projection = Type switch {
            CameraType.Perspective => Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegToRad * Fovy, _aspectRatio, MinDepth, MaxDepth),
            CameraType.Orthographic => Matrix4.CreateOrthographic(Size.X, Size.Y, MinDepth, MaxDepth),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void Render(params List<IDrawable> drawables)
    {
        foreach (var drawable in drawables)
        {
            drawable.Draw(this);
        }
    }

    public void SetViewportSize(int width, int height)
    {
        _viewportWidth = width;
        _viewportHeight = height;
        _aspectRatio = (float)width / height;
    }
}
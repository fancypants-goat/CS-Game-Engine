using OpenTK.Graphics.Vulkan;
using OpenTK.Mathematics;

namespace Engine;

public class Transform : Component
{
    private Vector3 _position;
    public Vector3 Position
    {
        get
        {
            return _position;
        }
        set
        {
            _position = value;
            TranslationMatrix = Matrix4.CreateTranslation(_position);
        }
    }

    private Vector3 _size;
    public Vector3 Size
    {
        get
        {
            return _size;
        }
        set
        {
            _size = value;
            SizeMatrix = Matrix4.CreateScale(_size);
        }
    }

    private Vector3 _rotation;
    public Vector3 Rotation
    {
        get
        {
            return _rotation;
        }
        set
        {
            _rotation = value;
            RotationMatrix = Matrix4.CreateRotationZ(MathHelper.DegToRad * _rotation.Z)
                             * Matrix4.CreateRotationY(MathHelper.DegToRad * _rotation.Y)
                             * Matrix4.CreateRotationX(MathHelper.DegToRad * _rotation.X);
            
            Forwards = new Vector3(
                (float)(Math.Cos(_rotation.X) * Math.Sin(_rotation.Y)),
                (float)(-Math.Sin(_rotation.X)),
                (float)(Math.Cos(_rotation.X) * Math.Cos(_rotation.Y)));
            
            Horizontal = new Vector3(Forwards.X, 0, Forwards.Z).Normalized();
            Right = Vector3.Cross(Vector3.UnitY, Forwards);
            Up = Vector3.Cross(Forwards, Right);
        }
    }


    private Matrix4 _translationMatrix;
    public Matrix4 TranslationMatrix
    {
        get
        {
            return _translationMatrix;
        }
        private set
        {
            _translationMatrix = value;
            SetModelMatrix();
        }
    }
    
    private Matrix4 _sizeMatrix;
    public Matrix4 SizeMatrix
    {
        get
        {
            return _sizeMatrix;
        }
        private set
        {
            _sizeMatrix = value;
            SetModelMatrix();
        }
    }
    
    private Matrix4 _rotationMatrix;
    public Matrix4 RotationMatrix
    {
        get
        {
            return _rotationMatrix;
        }
        private set
        {
            _rotationMatrix = value;
            SetModelMatrix();
        }
    }
    
    public Matrix4 ModelMatrix { get; private set; }
    
    public Vector3 Forwards { get; private set; }
    public Vector3 Right { get; private set; }
    public Vector3 Up { get; private set; }
    public Vector3 Horizontal { get; private set; }

    public Transform(Entity parent) : base(parent)
    {
        Position = new Vector3(0, 0, 0);
        Size = new Vector3(1, 1, 1);
        Rotation = new Vector3(0, 0, 0);
    }

    public Transform(Entity parent, Vector3 position, Vector3 size, Vector3 rotation) : base(parent)
    {
        Position = position;
    }
    
    private void SetModelMatrix()
    {
        ModelMatrix = _translationMatrix * _sizeMatrix * _rotationMatrix;
    }
    
    
    // TRANSFORMATIONS

    public void Translate(Vector3 v)
    {
        Position += v;
    }
    public void Translate(float x, float y, float z)
    {
        Position += new Vector3(x, y, z);
    }
    public void Translate(float v, Vector3 d)
    {
        Position += d * v;
    }
    
    public void Scale(Vector3 v)
    {
        Size *= v;
    }
    public void Scale(float x, float y, float z)
    {
        Size *= new Vector3(x, y, z);
    }

    public void Rotate(Vector3 v)
    {
        Rotation += v;
    }
    public void Rotate(float x, float y, float z)
    {
        Rotation += new Vector3(x, y, z);
    }
    public void Rotate(float v, Vector3 d)
    {
        Rotation += d * v;
    }
}
using System.Drawing;

namespace Engine;

/// <summary>
/// Material contains the Material data of a Mesh
/// </summary>
public struct Material : IDisposable
{
    public Shader Shader { get; set; }
    public Texture? Texture { get; set; }
    public Color Color { get; set; } // ambient color
    public Color DiffuseColor { get; set; }
    public Color SpecularColor { get; set; }
    public float SpecularExponent { get; set; }
    

    private bool _isDisposed = false;

    public Material()
    {
    }

    public void Use()
    {
        Shader.Use();
        Texture?.Use();
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        
        Shader.Dispose();
        Texture?.Dispose();
        
        _isDisposed = true;
    }
}
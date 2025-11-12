using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace Engine;

public class Texture : IDisposable
{
    public readonly int Handle;
    
    public int Width { get; private set; }
    public int Height { get; private set; }

    private bool _isDisposed;

    public Texture(string path, bool generateMipmaps)
    {
        ImageResult image = null;
        
        try
        {
            StbImage.stbi_set_flip_vertically_on_load(1);

            image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading image:", path, "|", e.Message, '\n', e);
            Dispose();
        }

        if (image == null) return;

        Width = image.Width;
        Height = image.Height;

        Handle = GL.GenTexture();
        Use();
        
        GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        
        if (generateMipmaps)
        {
            GL.GenerateMipmap(TextureTarget.Texture2d);
            
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }
        else
        {
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }
        GL.BindTexture(TextureTarget.Texture2d, 0);
    }

    public void Use()
    {
        GL.BindTexture(TextureTarget.Texture2d, Handle);
    }

    

    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            GL.BindTexture(TextureTarget.Texture2d, 0);
            GL.DeleteTexture(Handle);
        }

        _isDisposed = true;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    ~Texture()
    {
        if (_isDisposed) return;
        
        Debug.LogMemLeak("Texture");
        Dispose(false);
    }
}
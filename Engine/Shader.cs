using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGLES2;

namespace Engine;

public class Shader
{
    public int Handle { get; private set; }

    public Shader(string vertPath, string fragPath)
    {
        // read the sources as text
        var vertSource = File.ReadAllText(vertPath);
        var fragSource = File.ReadAllText(fragPath);
        
        // create the shaders from the shader sources
        var vertShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertShader, vertSource);
        var fragShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragShader, fragSource);


        GL.CompileShader(vertShader);
        ReportCompileStatus(vertShader);
        GL.CompileShader(fragShader);
        ReportCompileStatus(fragShader);
    }



    public int ReportCompileStatus(int shader)
    {
        GL.GetShaderi(shader, ShaderParameterName.CompileStatus, out int succes);
        if (succes == 0)
        {
            GL.GetShaderInfoLog(shader, out string log);
            Debug.LogAs(Debug.LogType.Error, "Failed to Compile Shader");
            Console.WriteLine(log);
        }

        return succes;
    }
}
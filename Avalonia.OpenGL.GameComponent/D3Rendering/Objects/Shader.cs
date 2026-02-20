namespace Avalonia.OpenGL.GameComponent.D3Rendering.Objects
{
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;
    using System;
    using System.IO;

    public class Shader
    {
        public int Handle { get; private set; }

        private bool _isDisposed = false;

        public static Shader FromFile(string vertexPath, string fragmentPath)
        {

            string vertexShaderSource = File.ReadAllText(vertexPath);
            string fragmentShaderSource = File.ReadAllText(fragmentPath);

            return new Shader(vertexShaderSource, fragmentShaderSource);
        }

        public Shader(string vertexShaderSource, string fragmentShaderSource)
        {
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            CheckCompileErrors(vertexShader, "VERTEX");

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            CheckCompileErrors(fragmentShader, "FRAGMENT");

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            GL.LinkProgram(Handle);
            CheckCompileErrors(Handle, "PROGRAM");

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public void SetMatrix4(string name, Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, false, ref matrix);
        }

        public void SetVector4(string name, Vector4 data)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform4(location, data.X, data.Y, data.Z, data.W);
        }
        public void SetVector3(string name, Vector3 data)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform3(location, data.X, data.Y, data.Z);
        }

        public void SetFloat(string name, float data)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform1(location, data);
        }

        private void CheckCompileErrors(int shader, string type)
        {
            if (type != "PROGRAM")
            {
                GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
                if (success == 0)
                {
                    string infoLog = GL.GetShaderInfoLog(shader);
                    System.Diagnostics.Debug.WriteLine($"ERROR::SHADER_COMPILATION_ERROR of type: {type}\n{infoLog}");
                }
            }
            else
            {
                GL.GetProgram(shader, GetProgramParameterName.LinkStatus, out int success);
                if (success == 0)
                {
                    string infoLog = GL.GetProgramInfoLog(shader);
                    System.Diagnostics.Debug.WriteLine($"ERROR::PROGRAM_LINKING_ERROR\n{infoLog}");
                }
            }
        }
        public void Dispose()
        {
            if (_isDisposed) return;

            GL.DeleteProgram(Handle);
            Handle = 0;

            _isDisposed = true;
            GC.SuppressFinalize(this);
        }

        ~Shader()
        {
            if (!_isDisposed) System.Diagnostics.Debug.WriteLine("MEMORY LEAK: Shader not disposed!");
        }

        #region Data

        public static string VertexShaderSource => _vertexShaderSource ??= File.ReadAllText("D3Rendering/Data/Shaders/VertexShader.glsl");
        static string? _vertexShaderSource;

        public static string FragmentShaderSource => _fragmentShaderSource ??= File.ReadAllText("D3Rendering/Data/Shaders/FragmentShader.glsl");
        static string? _fragmentShaderSource;

        public static Shader Default => _default ??= new Shader(VertexShaderSource, FragmentShaderSource);
        static Shader _default;


        public static string LitVertexShaderSource => _litVertexShaderSource ??= File.ReadAllText("D3Rendering/Data/Shaders/LitVertexShader.glsl");
        static string? _litVertexShaderSource;

        public static string LitFragmentShaderSource => _litFragmentShaderSource ??= File.ReadAllText("D3Rendering/Data/Shaders/LitFragmentShader.glsl");
        static string? _litFragmentShaderSource;

        public static Shader Lit => _lit ??= new Shader(LitVertexShaderSource, LitFragmentShaderSource);
        static Shader _lit;

        #endregion
    }

    public interface ILightingSource
    {
        Vector3 Direction { get; set; }
        Vector3 Color { get; set; }
        float AmbientStrength { get; set; }
    }

    public class DirectionalLight : BaseRenderedObject, ILightingSource
    {
        public Vector3 Direction { get; set; } = new Vector3(-0.5f, -1.0f, -0.5f).Normalized();

        public Vector3 Color { get; set; } = new Vector3(1.0f, 1.0f, 1.0f);

        public float AmbientStrength { get; set; } = 0.3f;

        public override event Action? TransformChanged;

        public override void Draw(Matrix4 view, Matrix4 projection) { }

        public override (Vector3 min, Vector3 max) GetBounds()
            => (Vector3.Zero, Vector3.Zero); 

        public override Matrix4 GetModelMatrix()
            => Matrix4.Identity;
    }
}

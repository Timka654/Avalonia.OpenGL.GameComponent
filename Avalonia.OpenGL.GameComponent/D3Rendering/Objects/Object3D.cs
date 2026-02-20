namespace Avalonia.OpenGL.GameComponent.D3Rendering.Objects
{
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;
    using System;

    public class Object3D : BaseRenderedObject, IDisposable
    {
        public Vector3 Position { get => position; set { position = value; UpdateModelMatrix(); } }
        public Vector3 Rotation { get => rotation; set { rotation = value; UpdateModelMatrix(); } }
        public Vector3 Scale { get => scale; set { scale = value; UpdateModelMatrix(); } }
        public PrimitiveType Type { get; init; } = PrimitiveType.Triangles;

        public override event Action? TransformChanged = () => { };

        public Vector4 Color { get; set; } = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);

        public Vector3 LocalMin { get; private set; }
        public Vector3 LocalMax { get; private set; }

        public bool LineSmoothing { get; set; }

        public ILightingSource? LightingSource { get => lightingSource; set => lightingSource = value; }

        public bool LightsEnabled { get; set; } = true;

        public bool HasNormals { get; }

        private float[] vertices;
        private uint[] indices;
        private readonly Shader shader;
        private int vao, vbo, ebo;

        private Matrix4 model;
        private Vector3 position = Vector3.Zero;
        private Vector3 rotation = Vector3.Zero;
        private Vector3 scale = Vector3.One;
        private ILightingSource? lightingSource;

        private bool _isDisposed = false;

        public Object3D(float[] vertices, uint[] indices, Shader shader, bool hasNormals = false)
        {
            this.vertices = vertices;
            this.indices = indices;
            this.shader = shader;
            this.HasNormals = hasNormals;

            CalculateLocalBounds();
            UpdateModelMatrix();
            SetupBuffers();
        }
        private void CalculateLocalBounds()
        {
            if (vertices == null || vertices.Length < 3)
            {
                LocalMin = Vector3.Zero;
                LocalMax = Vector3.Zero;
                return;
            }

            float minX = vertices[0], minY = vertices[1], minZ = vertices[2];
            float maxX = vertices[0], maxY = vertices[1], maxZ = vertices[2];

            int stride = HasNormals ? 6 : 3;

            for (int i = 0; i < vertices.Length; i += stride)
            {
                float x = vertices[i];
                float y = vertices[i + 1];
                float z = vertices[i + 2];

                if (x < minX) minX = x;
                if (x > maxX) maxX = x;

                if (y < minY) minY = y;
                if (y > maxY) maxY = y;

                if (z < minZ) minZ = z;
                if (z > maxZ) maxZ = z;
            }

            LocalMin = new Vector3(minX, minY, minZ);
            LocalMax = new Vector3(maxX, maxY, maxZ);
        }

        private void UpdateModelMatrix()
        {
            model = Matrix4.CreateScale(Scale) *
                            Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X)) *
                            Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y)) *
                            Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z)) *
                            Matrix4.CreateTranslation(Position);
            TransformChanged!();
        }
        private void SetupBuffers()
        {
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            int stride = HasNormals ? 6 * sizeof(float) : 3 * sizeof(float);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);

            if (HasNormals)
            {
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public override void Draw(Matrix4 view, Matrix4 projection)
        {
            shader.Use();


            shader.SetMatrix4("model", model * Parent?.GetModelMatrix() ?? Matrix4.Identity);
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);

            shader.SetVector4("uColor", Color);

            if (LineSmoothing)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                
                GL.Enable(EnableCap.LineSmooth);
            }

            GL.BindVertexArray(vao);

            if (LightsEnabled && HasNormals && LightingSource != null)
            {
                shader.SetVector3("lightDir", LightingSource.Direction);
                shader.SetVector3("lightColor", LightingSource.Color);
                shader.SetFloat("ambientStrength", LightingSource.AmbientStrength);
            }
            GL.DrawElements(Type, indices.Length, DrawElementsType.UnsignedInt, 0);

            if (LineSmoothing)
            {
                GL.Disable(EnableCap.Blend);
                GL.Disable(EnableCap.LineSmooth);
            }

            GL.BindVertexArray(0);

            foreach (var item in Childs)
            {
                item.Draw(view, projection);
            }
        }

        public override Matrix4 GetModelMatrix()
            => model;

        public override (Vector3 min, Vector3 max) GetBounds()
            => (LocalMin, LocalMax);

        public override void OnSceneSet(Scene? scene)
        {
            base.OnSceneSet(scene);

            if (LightsEnabled && HasNormals && scene != null)
                LightingSource ??= scene.DirectionalLight;
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            if (vao != 0) GL.DeleteVertexArray(vao);
            if (vbo != 0) GL.DeleteBuffer(vbo);
            if (ebo != 0) GL.DeleteBuffer(ebo);

            vao = 0; vbo = 0; ebo = 0;

            _isDisposed = true;
            GC.SuppressFinalize(this);
        }

        ~Object3D()
        {
            if (!_isDisposed)
            {
                System.Diagnostics.Debug.WriteLine("MEMORY LEAK: Object3D has GC collected. But Dispose() was not called!");
            }
        }
    }
}

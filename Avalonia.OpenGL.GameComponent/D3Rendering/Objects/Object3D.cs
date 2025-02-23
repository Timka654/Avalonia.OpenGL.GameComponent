namespace Avalonia.OpenGL.GameComponent.D3Rendering.Objects
{
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;

    public class Object3D : BaseRenderedObject
    {
        public Vector3 Position { get; set; } = Vector3.Zero;

        public Vector3 Rotation { get; set; } = Vector3.Zero;

        public Vector3 Scale { get; set; } = Vector3.One;

        private float[] vertices;
        private uint[] indices;
        private readonly Shader shader;
        private int vao, vbo, ebo;

        public Object3D(float[] vertices, uint[] indices, Shader shader)
        {
            this.vertices = vertices;
            this.indices = indices;
            this.shader = shader;
            SetupBuffers();
        }

        private void SetupBuffers()
        {
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            // Загружаем вершины
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // Загружаем индексы
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // Настройка атрибута вершин
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public override void Draw(Matrix4 view, Matrix4 projection)
        {
            shader.Use();

            Matrix4 model = Matrix4.CreateScale(Scale) *
                            Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X)) *
                            Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y)) *
                            Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z)) *
                            Matrix4.CreateTranslation(Position);

            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);

            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}

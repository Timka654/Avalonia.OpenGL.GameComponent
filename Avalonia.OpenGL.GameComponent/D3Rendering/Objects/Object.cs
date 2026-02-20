namespace Avalonia.OpenGL.GameComponent.D3Rendering.Objects
{
    using OpenTK.Mathematics;
    using System;

    public class Object : BaseRenderedObject
    {
        public override event Action? TransformChanged = ()=> { };

        public override void Draw(Matrix4 view, Matrix4 projection) { }

        public override (Vector3 min, Vector3 max) GetBounds()
            => (Vector3.Zero, Vector3.Zero);

        public override Matrix4 GetModelMatrix()
            => Matrix4.Identity;
    }
}

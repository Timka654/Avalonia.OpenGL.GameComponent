namespace Avalonia.OpenGL.GameComponent.D3Rendering.Objects
{
    using Avalonia.OpenGL.GameComponent.D3Rendering.Scripts;
    using Avalonia.OpenGL.GameComponent.Utils;
    using OpenTK.Mathematics;
    using System;
    using System.Collections.Generic;

    public class Scene : BaseRenderedObject
    {
        public OpenGL3DFrame? Frame { get; private set; }

        public event Action<OpenGL3DFrame?> OnUpdateFrame = e => { };
        public override event Action? TransformChanged;

        public IReadOnlyList<ICollider> Colliders => _colliders;

        public ILightingSource? DirectionalLight { get; set; }

        private List<ICollider> _colliders { get; } = new List<ICollider>();

        public ICollider? Raycast(Ray ray, out float hitDistance)
        {
            ICollider? closestCollider = null;
            hitDistance = float.MaxValue;

            foreach (var collider in _colliders)
            {
                if (collider.Raycast(ray, out float dist) && dist < hitDistance)
                {
                    hitDistance = dist;
                    closestCollider = collider;
                }
            }

            return closestCollider;
        }

        protected override void OnChildAdd(IRenderedObject child)
        {
            trySetCamera(child);

            child.CurrentScene = this;

            base.OnChildAdd(child);
        }

        private void trySetCamera(IRenderedObject obj)
        {
            if (obj is Camera c && CurrentCamera == default && c.Active)
                CurrentCamera = c;
        }

        public Camera? CurrentCamera { get; private set; }

        internal void UpdateFrame(OpenGL3DFrame? frame)
        {
            Frame = frame;
            OnUpdateFrame(frame);

            foreach (var item in Scripts)
            {
                item.SetParent(this);
            }
        }

        internal void UpdateScene()
        {
            foreach (var item in Scripts)
            {
                item.Update();
            }
        }

        internal void DrawScene()
        {
            if (CurrentCamera?.Script == default)
                return;

            var view = CurrentCamera.Script.GetViewMatrix();
            var projection = CurrentCamera.Script.Projection;

            foreach (var obj in Childs)
            {
                obj.Draw(view, projection);
            }
        }

        internal void RegisterCollider(BoxCollider boxCollider)
        {
            _colliders.Add(boxCollider);
        }

        internal void UnregisterCollider(BoxCollider boxCollider)
        {
            _colliders.Remove(boxCollider);
        }

        public override void Draw(Matrix4 view, Matrix4 projection)
        {
            throw new System.NotImplementedException();
        }

        public override Matrix4 GetModelMatrix()
            => Matrix4.Identity;

        public override (Vector3 min, Vector3 max) GetBounds()
        {
            throw new NotImplementedException();
        }
    }

}

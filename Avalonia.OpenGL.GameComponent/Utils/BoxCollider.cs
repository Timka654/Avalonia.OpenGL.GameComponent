using Avalonia.OpenGL.GameComponent.D3Rendering;
using Avalonia.OpenGL.GameComponent.D3Rendering.Objects;
using OpenTK.Mathematics;

namespace Avalonia.OpenGL.GameComponent.Utils
{
    public interface ICollider
    {
        bool Raycast(Ray ray, out float distance);

        IRenderedObject? Object { get; }
    }

    public class BoxCollider : ICollider, IScriptObject
    {
        public Vector3 BoundsMin { get; private set; }
        public Vector3 BoundsMax { get; private set; }
        public bool Active { get; set; }

        public IRenderedObject? Object => parent;

        private void RecalculateBounds()
        {
            Matrix4 modelMatrix = this.parent!.GetModelMatrix();

            var bounds = this.parent!.GetBounds();
            Vector3 LocalMin = bounds.min;
            Vector3 LocalMax = bounds.max;

            Vector3[] corners = {
            new Vector3(LocalMin.X, LocalMin.Y, LocalMin.Z),
            new Vector3(LocalMax.X, LocalMin.Y, LocalMin.Z),
            new Vector3(LocalMin.X, LocalMax.Y, LocalMin.Z),
            new Vector3(LocalMax.X, LocalMax.Y, LocalMin.Z),
            new Vector3(LocalMin.X, LocalMin.Y, LocalMax.Z),
            new Vector3(LocalMax.X, LocalMin.Y, LocalMax.Z),
            new Vector3(LocalMin.X, LocalMax.Y, LocalMax.Z),
            new Vector3(LocalMax.X, LocalMax.Y, LocalMax.Z)
        };

            Vector3 transformedCorner = Vector3.TransformPosition(corners[0], modelMatrix);
            Vector3 newMin = transformedCorner;
            Vector3 newMax = transformedCorner;

            for (int i = 1; i < 8; i++)
            {
                transformedCorner = Vector3.TransformPosition(corners[i], modelMatrix);
                newMin = Vector3.ComponentMin(newMin, transformedCorner);
                newMax = Vector3.ComponentMax(newMax, transformedCorner);
            }

            BoundsMin = newMin;
            BoundsMax = newMax;
        }

        public bool Raycast(Ray ray, out float distance)
        {
            return ray.IntersectsAABB(BoundsMin, BoundsMax, out distance);
        }

        IRenderedObject? parent = null;

        public void SetParent(IRenderedObject? parent)
        {
            if (this.parent != null)
            {
                this.parent.TransformChanged -= RecalculateBounds;
                this.parent.OnUpdateScene -= Parent_OnUpdateScene;
            }

            this.parent = parent;

            if (this.parent != null)
            {
                Scene? scene = this.parent.CurrentScene;

                this.parent.TransformChanged += RecalculateBounds;
                this.parent.OnUpdateScene += Parent_OnUpdateScene;

                RecalculateBounds();

                if (scene != null)
                    Parent_OnUpdateScene(scene);
            }

        }

        Scene? scene = null;

        private void Parent_OnUpdateScene(Scene? obj)
        {
            scene?.UnregisterCollider(this);

            scene = obj;

            scene?.RegisterCollider(this);
        }

        public void Update()
        {
        }
    }
}

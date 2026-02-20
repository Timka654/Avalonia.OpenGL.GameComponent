using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.OpenGL.GameComponent.Utils
{
    public struct Ray
    {
        public Vector3 Origin;
        public Vector3 Direction;

        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction.Normalized();
        }

        public bool IntersectsAABB(Vector3 boxMin, Vector3 boxMax, out float intersectionDistance)
        {
            intersectionDistance = 0f;

            Vector3 invDir = new Vector3(1.0f / Direction.X, 1.0f / Direction.Y, 1.0f / Direction.Z);

            float t1 = (boxMin.X - Origin.X) * invDir.X;
            float t2 = (boxMax.X - Origin.X) * invDir.X;
            float tmin = Math.Min(t1, t2);
            float tmax = Math.Max(t1, t2);

            float t3 = (boxMin.Y - Origin.Y) * invDir.Y;
            float t4 = (boxMax.Y - Origin.Y) * invDir.Y;
            tmin = Math.Max(tmin, Math.Min(t3, t4));
            tmax = Math.Min(tmax, Math.Max(t3, t4));

            float t5 = (boxMin.Z - Origin.Z) * invDir.Z;
            float t6 = (boxMax.Z - Origin.Z) * invDir.Z;
            tmin = Math.Max(tmin, Math.Min(t5, t6));
            tmax = Math.Min(tmax, Math.Max(t5, t6));

            if (tmax < 0) return false;

            if (tmin > tmax) return false;

            intersectionDistance = tmin;
            return true;
        }

        public static Ray ScreenPointToRay(Vector2 mousePosition, Vector2 viewportSize, ICamera camera)
        {
            float x = (2.0f * mousePosition.X) / viewportSize.X - 1.0f;
            float y = 1.0f - (2.0f * mousePosition.Y) / viewportSize.Y;
            float z = 1.0f;

            Vector4 rayClip = new Vector4(x, y, -1.0f, 1.0f);

            Vector4 rayEye = rayClip * Matrix4.Invert(camera.Projection);
            rayEye = new Vector4(rayEye.X, rayEye.Y, -1.0f, 0.0f);

            Matrix4 viewMatrix = Matrix4.LookAt(camera.Position, camera.Target, camera.Up);

            Vector3 rayWorld = (rayEye * Matrix4.Invert(viewMatrix)).Xyz;

            return new Ray(camera.Position, rayWorld);
        }
    }

    public interface ICamera
    {
        Vector3 Position { get; }
        Vector3 Target { get; }
        Vector3 Up { get; }
        Matrix4 Projection { get; }
    }
}

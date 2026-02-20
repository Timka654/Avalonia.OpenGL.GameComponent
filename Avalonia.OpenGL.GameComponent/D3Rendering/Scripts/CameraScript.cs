using Avalonia.Input;
using Avalonia.OpenGL.GameComponent.Utils;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

namespace Avalonia.OpenGL.GameComponent.D3Rendering.Scripts
{
    public class CameraScript : IScriptObject, ICamera
    {
        public Vector3 Position { get; private set; }

        public Vector3 Target { get; private set; } = Vector3.Zero;

        public Vector3 Up { get; private set; } = Vector3.UnitY;

        public float MinDistance { get; set; } = 2.0f;
        public float MaxDistance { get; set; } = 200.0f;

        public Matrix4 Projection { get; private set; } = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45.0f),
                1.5f,
                0.1f,
                100.0f
            );

        public bool Active { get; set; } = true;

        private float yaw = -90f; 
        private float pitch = 0f; 
        private float distance = 5f; 

        private Vector2 lastMousePos;
        private bool firstMouse = true;

        private float sensitivity = 0.1f; 
        private float speed = 0.4f; 

        public CameraScript(Vector3 target, float startDistance)
        {
            Target = target;
            distance = startDistance;
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            float radYaw = MathHelper.DegreesToRadians(yaw);
            float radPitch = MathHelper.DegreesToRadians(pitch);

            Vector3 direction = new Vector3(
                MathF.Cos(radYaw) * MathF.Cos(radPitch),
                MathF.Sin(radPitch),
                MathF.Sin(radYaw) * MathF.Cos(radPitch)
            );

            Position = Target - direction * distance;
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Target, Up);
        }

        public void ReleaseMouse()
        {
            firstMouse = true;
            lastMousePos = default;
        }

        public void ProcessMouseMovement(Vector2 mousePos)
        {
            if (firstMouse)
            {
                lastMousePos = mousePos;
                firstMouse = false;
            }

            float offsetX = (mousePos.X - lastMousePos.X) * sensitivity;
            float offsetY = (lastMousePos.Y - mousePos.Y) * sensitivity; 

            lastMousePos = mousePos;

            yaw += offsetX;
            pitch = MathHelper.Clamp(pitch + offsetY, -89f, 89f); 

            UpdatePosition();
        }

        public void ProcessTarget(Vector2 mousePos)
        {
            if (firstMouse)
            {
                lastMousePos = mousePos;
                firstMouse = false;
            }

            float offsetX = (mousePos.X - lastMousePos.X) * sensitivity;
            float offsetY = (lastMousePos.Y - mousePos.Y) * sensitivity; 

            lastMousePos = mousePos;
            
            
            Vector3 forward = (Target - Position).Normalized();
            Vector3 right = Vector3.Cross(forward, Up).Normalized();
            Vector3 localUp = Vector3.Cross(right, forward).Normalized();

            
            
            Target -= right * offsetX;
            Target -= localUp * offsetY;

            UpdatePosition();
        }

        public void ProcessScroll(float offset)
        {
            distance = MathHelper.Clamp(distance - offset * 0.5f, MinDistance, MaxDistance);
            UpdatePosition();
        }

        private void moveUp(Vector3 pos)
        {
            Target += pos;

            Position += pos;
        }

        private void moveDown(Vector3 pos)
        {
            Target -= pos;

            Position -= pos;
        }

        public void ProcessKeyboard(PhysicalKey key)
        {
            Vector3 forward = (Target - Position).Normalized();
            
            Vector3 right = Vector3.Cross(forward, Up).Normalized();
            
            Vector3 actualUp = Vector3.Cross(right, forward).Normalized();

            if (key == PhysicalKey.W) moveUp( forward * speed);
            if (key == PhysicalKey.S) moveDown( forward * speed);
            if (key == PhysicalKey.A) moveDown( right * speed);
            if (key == PhysicalKey.D) moveUp( right * speed);

            
            if (key == PhysicalKey.Q) moveUp( actualUp * speed);
            if (key == PhysicalKey.E) moveDown( actualUp * speed);

            UpdatePosition();
        }

        OpenGL3DFrame? _frame;

        public void SetFrame(OpenGL3DFrame? frame)
        {
            _frame = frame;
        }

        public void Update()
        {

        }

        public void SetParent(IRenderedObject? parent)
        {
        }
    }

}

using Avalonia.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

namespace Avalonia.OpenGL.GameComponent.D3Rendering.Scripts
{
    public class CameraScript : IScriptObject
    {
        public Vector3 Position { get; private set; }

        public Vector3 Target { get; private set; } = Vector3.Zero;
        
        public Vector3 Up { get; private set; } = Vector3.UnitY;
        
        public Matrix4 Projection { get; private set; } = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45.0f),
                1.5f,
                0.1f,
                100.0f
            );

        public bool Active { get; set; } = true;

        private float yaw = -90f; // Угол поворота по горизонтали
        private float pitch = 0f; // Угол поворота по вертикали
        private float distance = 5f; // Расстояние от объекта (отдаление/приближение)

        private Vector2 lastMousePos;
        private bool firstMouse = true;

        private float sensitivity = 0.1f; // Чувствительность мыши
        private float speed = 0.1f; // Скорость перемещения

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
            float offsetY = (lastMousePos.Y - mousePos.Y) * sensitivity; // Инвертируем ось Y

            lastMousePos = mousePos;

            yaw += offsetX;
            pitch = MathHelper.Clamp(pitch + offsetY, -89f, 89f); // Ограничиваем наклон

            UpdatePosition();
        }

        public void ProcessScroll(float offset)
        {
            distance = MathHelper.Clamp(distance - offset * 0.5f, 2f, 20f);
            UpdatePosition();
        }

        public void ProcessKeyboard(PhysicalKey key)
        {
            Vector3 forward = (Target - Position).Normalized();
            Vector3 right = Vector3.Cross(forward, Up).Normalized();

            if (key == PhysicalKey.W) Target += forward * speed;
            if (key == PhysicalKey.S) Target -= forward * speed;
            if (key == PhysicalKey.A) Target -= right * speed;
            if (key == PhysicalKey.D) Target += right * speed;
            if (key == PhysicalKey.Q) Target += Up * speed; // Вверх
            if (key == PhysicalKey.E) Target -= Up * speed; // Вниз

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

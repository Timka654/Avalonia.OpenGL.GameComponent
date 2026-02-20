using Avalonia.Animation;
using Avalonia.Input;
using Avalonia.OpenGL.GameComponent.D3Rendering.Objects;
using Avalonia.OpenGL.GameComponent.Views;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.OpenGL.GameComponent.D3Rendering.Scripts
{
    class CameraControlScript : IScriptObject
    {
        MouseCameraControlEnum mode = MouseCameraControlEnum.None;

        bool movementPress = false;
        bool positionPress = false;

        public bool Active { get; set; } = true;

        protected void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(_frame);

            if (!movementPress && !positionPress)
            {
                movementPress = point.Properties.IsLeftButtonPressed;

                positionPress = point.Properties.IsRightButtonPressed;

                _camera.ProcessMouseMovement(new Vector2((float)point.Position.X, (float)point.Position.Y));
                _camera.ProcessTarget(new Vector2((float)point.Position.X, (float)point.Position.Y));
            }
        }

        protected void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            var point = e.GetCurrentPoint(_frame);

            if (movementPress)
                movementPress = point.Properties.IsLeftButtonPressed;

            if (positionPress)
                positionPress = point.Properties.IsRightButtonPressed;

            if (!movementPress && !positionPress)
                _camera.ReleaseMouse();

        }

        protected void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (movementPress)
            {
                var mousePos = e.GetPosition(_frame);

                _camera.ProcessMouseMovement(new Vector2((float)mousePos.X, (float)mousePos.Y));
            }
            if (positionPress)
            {
                var mousePos = e.GetPosition(_frame);

                _camera.ProcessTarget(new Vector2((float)mousePos.X, (float)mousePos.Y));
            }
        }

        protected void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            _camera.ProcessScroll((float)e.Delta.Y);
        }

        public void SetParent(IRenderedObject? parent)
        {
            this.parent = parent;
            _camera = parent.GetScript<CameraScript>();
            parent.OnUpdateScene += Parent_OnUpdateScene;
            CurrentScene_OnUpdateFrame(parent.CurrentScene?.Frame);
        }

        private void Parent_OnUpdateScene(Objects.Scene? obj)
        {
            if (_scene != null && obj == null)
            {
                _scene.OnUpdateFrame -= CurrentScene_OnUpdateFrame;
            }

            _scene = obj;

            if (_scene != null)
            {
                _frame = _scene.Frame;
                _scene.OnUpdateFrame += CurrentScene_OnUpdateFrame;

            }
        }

        private void CurrentScene_OnUpdateFrame(OpenGL3DFrame? frame)
        {
            if (frame == null && _frame != null)
            {
                _frame.KeyDown -= OnKeyDown;

                _frame.PointerPressed -= OnPointerPressed;
                _frame.PointerReleased -= OnPointerReleased;

                _frame.PointerMoved -= OnPointerMoved;

                _frame.PointerWheelChanged -= OnPointerWheelChanged;
            }

            _frame = frame;

            if (_frame != null)
            {
                _frame.KeyDown += OnKeyDown;

                _frame.PointerPressed += OnPointerPressed;
                _frame.PointerReleased += OnPointerReleased;

                _frame.PointerMoved += OnPointerMoved;

                _frame.PointerWheelChanged += OnPointerWheelChanged;
            }
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            _camera.ProcessKeyboard(e.PhysicalKey);
        }

        CameraScript _camera;
        IRenderedObject? parent;

        OpenGL3DFrame? _frame;
        Scene _scene;


        public void Update()
        {
        }
    }

    public enum MouseCameraControlEnum
    {
        None,
        Move,
        Rotation
    }
}

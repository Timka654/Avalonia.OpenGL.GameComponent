using Avalonia.Input;
using Avalonia.OpenGL.GameComponent.D3Rendering;
using Avalonia.OpenGL.GameComponent.D3Rendering.Objects;
using Avalonia.OpenGL.GameComponent.D3Rendering.Scripts;
using Avalonia.OpenGL.GameComponent.Utils;
using OpenTK.Mathematics;
using System;

namespace Avalonia.OpenGL.GameComponent.Editor
{
    public class MouseRayPickScript : IScriptObject
    {
        public bool Active { get; set; }

        public event Action<BaseRenderedObject?> OnObjectSelected = e => { };

        private CameraScript? _camera;

        public void SetParent(IRenderedObject? parent)
        {
            _camera = parent?.GetScript<CameraScript>();
            if (parent != null)
            {
                parent.OnUpdateScene += Parent_OnUpdateScene;
                CurrentScene_OnUpdateFrame(parent.CurrentScene?.Frame);
            }
        }

        Scene? _scene;
        OpenGL3DFrame? _frame;
        private void Parent_OnUpdateScene(Scene? obj)
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
                _frame.PointerPressed -= OnPointerPressed;
            }

            _frame = frame;

            if (_frame != null)
            {
                _frame.PointerPressed += OnPointerPressed;
            }
        }

        protected void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(_frame);

            
            if (point.Properties.IsLeftButtonPressed)
            {
                Vector2 mousePos = new Vector2((float)point.Position.X, (float)point.Position.Y);
                Vector2 viewport = new Vector2((float)_frame.Bounds.Width, (float)_frame.Bounds.Height);

                Ray ray = Ray.ScreenPointToRay(mousePos, viewport, _camera);

                var c = _scene.Raycast(ray, out float distance);

                
                if (c?.Object != null && c.Object is BaseRenderedObject obj)
                {
                    OnObjectSelected(obj); 
                }
                else
                {
                    OnObjectSelected(null);
                }
            }
        }


        public void Update()
        {
        }
    }
}

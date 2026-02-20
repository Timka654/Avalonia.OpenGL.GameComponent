using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.OpenGL.GameComponent.D3Rendering.Objects;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTKAvalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.OpenGL.GameComponent.D3Rendering
{
    public class OpenGL3DFrame : BaseTkOpenGlControl
    {
        public event Action<OpenGL3DFrame> OnInitializedFrame = frame => { };

        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);

            Width = e.NewSize.Width;
            Height = e.NewSize.Height;

            if (!inited)
                return;

            buildViewMatrix();
        }

        private void buildViewMatrix()
        {
            GL.Viewport(0, 0, (int)Width, (int)Height);
        }

        bool inited = false;

        public Shader BasicRenderShared { get; private set; }

        public IEnumerable<Scene> ActiveScenes => _activeScenes.ToArray();

        private List<Scene> _activeScenes = new List<Scene>();

        public void AddScene(Scene scene)
        {
            if (!inited)
                throw new OpenGL3DFrameNotInitializedException();

            scene.UpdateFrame(this);

            _activeScenes.Add(scene);
        }

        public void RemoveScene(Scene scene)
        {
            if (_activeScenes.Remove(scene))
            {
                scene.UpdateFrame(null);
            }
        }

        public OpenGL3DFrameTime Time { get; } = new OpenGL3DFrameTime();

        protected override void OpenTkInit()
        {
            BasicRenderShared ??= Shader.Default;

            GL.ClearColor(0.1f, 0.5f, 1.0f, 0); 

            GL.Enable(EnableCap.DepthTest);

            buildViewMatrix();
            
            inited = true;

            OnInitializedFrame(this);
        }

        protected override void OnOpenGlLost()
        {
        }

        protected override void OpenTkRender()
        {
            Time.StartFrame();

            if (!IsEnabled)
                return;

            foreach (var item in _activeScenes)
            {
                item.UpdateScene();
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (var item in _activeScenes)
            {
                item.DrawScene();
            }
        }
    }
}

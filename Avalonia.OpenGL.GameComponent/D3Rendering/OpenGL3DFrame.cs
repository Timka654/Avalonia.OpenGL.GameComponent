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


        // A simple vertex shader possible. Just passes through the position vector.
        const string VertexShaderSource = """
            #version 330 core
            layout (location = 0) in vec3 aPos;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;

            void main()
            {
                gl_Position = projection * view * model * vec4(aPos, 1.0);
            }
            """;

        // A simple fragment shader. Just a constant red color.
        const string FragmentShaderSource = """
            #version 330 core
            out vec4 FragColor;

            void main()
            {
                FragColor = vec4(1.0, 0.5, 0.2, 1.0);
            }
            """;

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
            BasicRenderShared ??= new Shader(VertexShaderSource, FragmentShaderSource);


            GL.ClearColor(0.1f, 0.5f, 1.0f, 0);

            buildViewMatrix();
            
            inited = true;

            OnInitializedFrame(this);
        }

        //private Scene currentScene = new Scene();

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

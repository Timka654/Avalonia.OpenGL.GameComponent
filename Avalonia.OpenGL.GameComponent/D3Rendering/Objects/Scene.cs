namespace Avalonia.OpenGL.GameComponent.D3Rendering.Objects
{
    using Avalonia.OpenGL.GameComponent.D3Rendering.Scripts;
    using OpenTK.Mathematics;
    using System;
    using System.Collections.Generic;

    public class Scene : BaseRenderedObject
    {
        public OpenGL3DFrame? Frame { get; private set; }

        public event Action<OpenGL3DFrame?> OnUpdateFrame = e => { };

        public override void OnChildAdd(IRenderedObject child)
        {
            trySetCamera(child);

            child.CurrentScene = this;

            base.OnChildAdd(child);
        }

        public override void OnChildRemove(IRenderedObject child)
        {
            base.OnChildRemove(child);
        }

        private void trySetCamera(IRenderedObject obj)
        {
            if (obj is Camera c)
            {
                if (CurrentCamera == default && c.Active)
                {
                    CurrentCamera = c;
                }
            }
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

        public override void Draw(Matrix4 view, Matrix4 projection)
        {
            throw new System.NotImplementedException();
        }
    }

}

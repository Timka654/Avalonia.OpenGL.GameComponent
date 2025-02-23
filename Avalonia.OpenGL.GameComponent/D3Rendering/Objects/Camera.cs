using Avalonia.OpenGL.GameComponent.D3Rendering.Scripts;

namespace Avalonia.OpenGL.GameComponent.D3Rendering.Objects
{
    public class Camera : Object
    {
        private CameraScript? script;

        public CameraScript? Script
        {
            get => script;
            set
            {
                var s = script;
                script = value;
                if (s != null) RemoveScript(s);
                if (script != null) AddScript(script);
            }
        }

    }
}

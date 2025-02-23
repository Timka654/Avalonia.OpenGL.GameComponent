using System;

namespace Avalonia.OpenGL.GameComponent.D3Rendering
{
    public class OpenGL3DFrameTime
    {
        public float DeltaTime { get; private set; }

        private DateTime? dt;
        
        internal void StartFrame()
        {
            var d = dt;

            dt =  DateTime.UtcNow;

            if (d != null)
                DeltaTime = (float)(dt.Value - d.Value).TotalMilliseconds;
        }
    }
}

using Avalonia.OpenGL.GameComponent.D3Rendering.Objects;

namespace Avalonia.OpenGL.GameComponent.D3Rendering
{
    public interface IScriptObject
    {
        bool Active { get; set; }

        void SetParent(IRenderedObject? parent);

        void Update();
    }
}

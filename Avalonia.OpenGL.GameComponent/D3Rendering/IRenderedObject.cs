using Avalonia.OpenGL.GameComponent.D3Rendering.Objects;
using Avalonia.OpenGL.GameComponent.Utils;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Avalonia.OpenGL.GameComponent.D3Rendering
{
    public interface IRenderedObject
    {
        bool Active { get; set; }

        internal Scene? CurrentScene { get; set; }

        event Action<Scene?> OnUpdateScene;


        IRenderedObject? Parent { get; }

        IReadOnlyList<IRenderedObject> Childs { get; }

        void AddChild(IRenderedObject child);

        void RemoveChild(IRenderedObject child);


        IReadOnlyList<IScriptObject> Scripts { get; }

        void AddScript<TScript>() where TScript : IScriptObject, new();

        void AddScript<TScript>(Func<TScript> getter) where TScript : IScriptObject;

        void AddScript(IScriptObject script);

        TScript? GetScript<TScript>() where TScript : IScriptObject;
        
        void RemoveScript(IScriptObject script);

        void Draw(Matrix4 view, Matrix4 projection);

        event Action? TransformChanged;

        Matrix4 GetModelMatrix();
        (Vector3 min, Vector3 max) GetBounds();
    }
}

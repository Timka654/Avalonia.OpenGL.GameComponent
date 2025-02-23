using Avalonia.OpenGL.GameComponent.D3Rendering.Objects;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Avalonia.OpenGL.GameComponent.D3Rendering
{
    public abstract class BaseRenderedObject : IRenderedObject
    {
        public bool Active { get; set; } = true;

        private Scene? _currentScene;

        public IReadOnlyList<IScriptObject> Scripts => _scripts;

        public IReadOnlyList<IRenderedObject> Childs => _childs;

        Scene? IRenderedObject.CurrentScene
        {
            get => _currentScene; set
            {
                _currentScene = value;

                foreach (var item in _childs)
                {
                    item.CurrentScene = value;
                }

                OnSceneSet(value);

                OnUpdateScene(value);
            }
        }

        public event Action<Scene?> OnUpdateScene = e => { };

        public Scene? GetScene()
            => _currentScene;

        public virtual void OnSceneSet(Scene? scene)
        { 
        
        }

        List<IScriptObject> _scripts = new List<IScriptObject>();
        List<IRenderedObject> _childs = new List<IRenderedObject>();

        public void AddScript<TScript>() where TScript : IScriptObject, new()
        {
            AddScript(new TScript());
        }

        public void AddScript<TScript>(Func<TScript> getter) where TScript : IScriptObject
        {
            AddScript(getter());
        }

        public void AddScript(IScriptObject script)
        {
            script.SetParent(this);
            _scripts.Add(script);
            OnScriptAdd(script);
        }

        public virtual void OnScriptAdd(IScriptObject script) { }

        public abstract void Draw(Matrix4 view, Matrix4 projection);

        public void RemoveScript(IScriptObject script)
        { if (_scripts.Remove(script)) { OnScriptRemove(script); script.SetParent(null); } }

        public virtual void OnScriptRemove(IScriptObject script) { }

        public TScript? GetScript<TScript>() where TScript : IScriptObject
            => (TScript?)Scripts.SingleOrDefault(x => x is TScript);

        public void AddChild(IRenderedObject child)
        {
            child.CurrentScene = _currentScene;
            _childs.Add(child);
            OnChildAdd(child);
        }

        public void RemoveChild(IRenderedObject child)
        { if (_childs.Remove(child)) { child.CurrentScene = null; OnChildRemove(child); } }

        public virtual void OnChildAdd(IRenderedObject child) { }

        public virtual void OnChildRemove(IRenderedObject child) { }
    }
}

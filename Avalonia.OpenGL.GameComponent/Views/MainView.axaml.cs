using Avalonia.Controls;
using Avalonia.OpenGL.GameComponent.D3Rendering;
using Avalonia.OpenGL.GameComponent.D3Rendering.Objects;
using Avalonia.OpenGL.GameComponent.D3Rendering.Scripts;
using Avalonia.OpenGL.GameComponent.Editor;
using Avalonia.OpenGL.GameComponent.Utils;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Avalonia.OpenGL.GameComponent.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        renderFrame.OnInitializedFrame += RenderFrame_OnInitializedFrame;
    }
    MouseRayPickScript mouseRayPickScript;
    private void RenderFrame_OnInitializedFrame(D3Rendering.OpenGL3DFrame obj)
    {
        var scene = new Scene();

        obj.AddScene(scene);

        var sun = new DirectionalLight()
        {
            Direction = new Vector3(-1f, -1f, 0f), 
            Color = new Vector3(1.0f, 1.0f, 1.0f),
            AmbientStrength = 0.2f
        };

        scene.AddChild(sun);
        scene.DirectionalLight = sun;

        var cam = new Camera()
        {
            Script = new CameraScript(new Vector3(0, 0, -5), 30)
        };

        cam.AddScript<CameraControlScript>();
        cam.AddScript(mouseRayPickScript = new MouseRayPickScript());

        mouseRayPickScript.OnObjectSelected += MouseRayPickScript_OnObjectSelected;

        scene.AddChild(cam);


        var cube = new Cube3DWithNormals();
        cube.Position = new Vector3(0, 0, -5);
        cube.AddScript<BoxCollider>();
        scene.AddChild(cube);


        var cube2 = new Cube3D();
        cube2.Position = new Vector3(0, 0, -25);
        cube2.AddScript<BoxCollider>();
        //cube2.AddChild(cube); // must thrown
        scene.AddChild(cube2);


        var cubeRel = new Cube3DWithNormals();
        cubeRel.Position = new Vector3(0, 0, -5);
        cubeRel.Color = new Vector4(1.0f, 0f, 1.0f, 0.8f);
        cubeRel.AddScript<BoxCollider>();
        cube2.AddChild(cubeRel);


        var gridData = GenerateGrid(500, 5);

        Object3D grid = new Object3D(gridData.vertices, gridData.indices, Shader.Default)
        {
            Type = OpenTK.Graphics.OpenGL.PrimitiveType.Lines,
            Color = new Vector4(1.0f, 1.0f, 1.0f, 0.2f)
        };

        grid.Position = new Vector3(0, 0, -5);

        scene.AddChild(grid);

        float[] axisVertices = {
            -500f, 0.01f, 0f,   500f, 0.01f, 0f,
             0f, 0.01f, -500f,  0f, 0.01f, 500f
        };

        uint[] axisIndices = { 0, 1, 2, 3 };

        Object3D axes = new Object3D(axisVertices, axisIndices, obj.BasicRenderShared)
        {
            Type = OpenTK.Graphics.OpenGL.PrimitiveType.Lines,
            Color = new Vector4(1.0f, 1.0f, 1.0f, 0.6f)
        };
        scene.AddChild(axes);
    }

    private BaseRenderedObject? _selectedObject;

    private void MouseRayPickScript_OnObjectSelected(BaseRenderedObject? obj)
    {
        bool visible = false;

        _selectedObject = obj;

        if (obj != null)
        {
            if (obj is Object3D o3d)
            {
                visible = true;

                PosXTextBox.Text = o3d.Position.X.ToString("F2");
                PosYTextBox.Text = o3d.Position.Y.ToString("F2");
                PosZTextBox.Text = o3d.Position.Z.ToString("F2");
            }
        }

        InspectorPanel.IsVisible = visible;
    }

    private void OnPositionChanged(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.Enter && _selectedObject != null)
        {
            if (float.TryParse(PosXTextBox.Text, out float x) &&
                float.TryParse(PosYTextBox.Text, out float y) &&
                float.TryParse(PosZTextBox.Text, out float z))
            {
                if (_selectedObject is Object3D o3d)
                    o3d.Position = new Vector3(x, y, z);
            }
        }
    }

    public (float[] vertices, uint[] indices) GenerateGrid(int size, float step)
    {
        var vertices = new List<float>();
        var indices = new List<uint>();
        uint currentIndex = 0;

        for (float i = -size; i <= size; i += step)
        {
            AddLine(new Vector3(i, 0, -size), new Vector3(i, 0, size), vertices, indices, ref currentIndex);
            AddLine(new Vector3(-size, 0, i), new Vector3(size, 0, i), vertices, indices, ref currentIndex);
        }

        return (vertices.ToArray(), indices.ToArray());
    }

    private void AddLine(Vector3 start, Vector3 end, List<float> v, List<uint> ind, ref uint idx)
    {
        // Только координаты (X, Y, Z)
        v.AddRange(new[] { start.X, start.Y, start.Z });
        v.AddRange(new[] { end.X, end.Y, end.Z });
        ind.Add(idx++);
        ind.Add(idx++);
    }
}

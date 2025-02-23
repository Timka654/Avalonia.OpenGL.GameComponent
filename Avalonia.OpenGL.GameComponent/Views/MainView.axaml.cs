using Avalonia.Controls;
using Avalonia.OpenGL.GameComponent.D3Rendering.Objects;
using Avalonia.OpenGL.GameComponent.D3Rendering.Scripts;
using OpenTK.Mathematics;

namespace Avalonia.OpenGL.GameComponent.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        renderFrame.OnInitializedFrame += RenderFrame_OnInitializedFrame;
    }

    private void RenderFrame_OnInitializedFrame(D3Rendering.OpenGL3DFrame obj)
    {
        float[] cubeVertices = {
                -0.5f, -0.5f, -0.5f,
                 0.5f, -0.5f, -0.5f,
                 0.5f,  0.5f, -0.5f,
                -0.5f,  0.5f, -0.5f,
                -0.5f, -0.5f,  0.5f,
                 0.5f, -0.5f,  0.5f,
                 0.5f,  0.5f,  0.5f,
                -0.5f,  0.5f,  0.5f
            };

        uint[] cubeIndices = {
                0, 1, 2, 2, 3, 0,
                1, 5, 6, 6, 2, 1,
                5, 4, 7, 7, 6, 5,
                4, 0, 3, 3, 7, 4,
                3, 2, 6, 6, 7, 3,
                4, 5, 1, 1, 0, 4
            };


        Object3D cube = new Object3D(cubeVertices, cubeIndices, obj.BasicRenderShared);
        cube.Position = new Vector3(0, 0, -5);

        var scene = new Scene();

        var cam = new Camera() { Script = new CameraScript(new Vector3(0, 0, -5), 30) };
        cam.AddScript<CameraControlScript>();

        scene.AddChild(cam);

        obj.AddScene(scene);

        scene.AddChild(cube);
    }
}

namespace Avalonia.OpenGL.GameComponent.D3Rendering.Objects
{
    public class Cube3D : Object3D
    {
        public Cube3D() : base(cubeVertices, cubeIndices, Shader.Default)
        {

        }

        public Cube3D(Shader shader) : base(cubeVertices, cubeIndices, shader)
        {

        }

        static readonly float[] cubeVertices = {
                -0.5f, -0.5f, -0.5f,
                 0.5f, -0.5f, -0.5f,
                 0.5f,  0.5f, -0.5f,
                -0.5f,  0.5f, -0.5f,
                -0.5f, -0.5f,  0.5f,
                 0.5f, -0.5f,  0.5f,
                 0.5f,  0.5f,  0.5f,
                -0.5f,  0.5f,  0.5f
            };

        static readonly uint[] cubeIndices = {
                0, 1, 2, 2, 3, 0,
                1, 5, 6, 6, 2, 1,
                5, 4, 7, 7, 6, 5,
                4, 0, 3, 3, 7, 4,
                3, 2, 6, 6, 7, 3,
                4, 5, 1, 1, 0, 4
            };
    }

    public class Cube3DWithNormals : Object3D
    {
        public Cube3DWithNormals() : base(cubeVertices, cubeIndices, Shader.Lit, true)
        {

        }

        public Cube3DWithNormals(Shader shader) : base(cubeVertices, cubeIndices, shader, true)
        {

        }

        static readonly float[] cubeVertices = {

            -0.5f, -0.5f,  0.5f,   0.0f,  0.0f,  1.0f,
             0.5f, -0.5f,  0.5f,   0.0f,  0.0f,  1.0f,
             0.5f,  0.5f,  0.5f,   0.0f,  0.0f,  1.0f,
            -0.5f,  0.5f,  0.5f,   0.0f,  0.0f,  1.0f,

            -0.5f, -0.5f, -0.5f,   0.0f,  0.0f, -1.0f,
             0.5f, -0.5f, -0.5f,   0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,   0.0f,  0.0f, -1.0f,
            -0.5f,  0.5f, -0.5f,   0.0f,  0.0f, -1.0f,

            -0.5f,  0.5f,  0.5f,  -1.0f,  0.0f,  0.0f,
            -0.5f,  0.5f, -0.5f,  -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f,  -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f,  0.5f,  -1.0f,  0.0f,  0.0f,

             0.5f,  0.5f,  0.5f,   1.0f,  0.0f,  0.0f,
             0.5f,  0.5f, -0.5f,   1.0f,  0.0f,  0.0f,
             0.5f, -0.5f, -0.5f,   1.0f,  0.0f,  0.0f,
             0.5f, -0.5f,  0.5f,   1.0f,  0.0f,  0.0f,

            -0.5f, -0.5f, -0.5f,   0.0f, -1.0f,  0.0f,
             0.5f, -0.5f, -0.5f,   0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,   0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f,  0.5f,   0.0f, -1.0f,  0.0f,

            -0.5f,  0.5f, -0.5f,   0.0f,  1.0f,  0.0f,
             0.5f,  0.5f, -0.5f,   0.0f,  1.0f,  0.0f,
             0.5f,  0.5f,  0.5f,   0.0f,  1.0f,  0.0f,
            -0.5f,  0.5f,  0.5f,   0.0f,  1.0f,  0.0f
        };

        static readonly uint[] cubeIndices = {
            0, 1, 2, 2, 3, 0,       
            4, 5, 6, 6, 7, 4,       
            8, 9, 10, 10, 11, 8,    
            12, 13, 14, 14, 15, 12, 
            16, 17, 18, 18, 19, 16, 
            20, 21, 22, 22, 23, 20  
        };
    }
}

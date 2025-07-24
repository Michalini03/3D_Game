using Game3D;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenTK.Graphics.OpenGL.GL;

namespace Zpg.models
{
    public class Wall : Model
    {
        public Wall(Vector3 position)
        {
            this.position = position;

            float tiling = 2.0f; // Adjust this to control how many times the texture repeats per face

            Vertex[] vertices = new Vertex[]
            {
            // Bottom face (Y-)
            new Vertex(new Vector3(-1.0f, 0.0f, -1.0f), new Vector3(0, -1, 0), new Vector2(0, 0)),
            new Vertex(new Vector3(1.0f, 0.0f, -1.0f), new Vector3(0, -1, 0), new Vector2(tiling, 0)),
            new Vertex(new Vector3(1.0f, 0.0f, 1.0f), new Vector3(0, -1, 0), new Vector2(tiling, tiling)),
            new Vertex(new Vector3(-1.0f, 0.0f, 1.0f), new Vector3(0, -1, 0), new Vector2(0, tiling)),

            // Top face (Y+)
            new Vertex(new Vector3(-1.0f, 3.0f, -1.0f), new Vector3(0, 1, 0), new Vector2(0, 0)),
            new Vertex(new Vector3(1.0f, 3.0f, -1.0f), new Vector3(0, 1, 0), new Vector2(tiling, 0)),
            new Vertex(new Vector3(1.0f, 3.0f, 1.0f), new Vector3(0, 1, 0), new Vector2(tiling, tiling)),
            new Vertex(new Vector3(-1.0f, 3.0f, 1.0f), new Vector3(0, 1, 0), new Vector2(0, tiling)),

            // Back face (Z-)
            new Vertex(new Vector3(-1.0f, 0.0f, -1.0f), new Vector3(0, 0, -1), new Vector2(0, 0)),
            new Vertex(new Vector3(1.0f, 0.0f, -1.0f), new Vector3(0, 0, -1), new Vector2(tiling, 0)),
            new Vertex(new Vector3(1.0f, 3.0f, -1.0f), new Vector3(0, 0, -1), new Vector2(tiling, tiling)),
            new Vertex(new Vector3(-1.0f, 3.0f, -1.0f), new Vector3(0, 0, -1), new Vector2(0, tiling)),

            // Front face (Z+)
            new Vertex(new Vector3(1.0f, 0.0f, 1.0f), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new Vertex(new Vector3(-1.0f, 0.0f, 1.0f), new Vector3(0, 0, 1), new Vector2(tiling, 0)),
            new Vertex(new Vector3(-1.0f, 3.0f, 1.0f), new Vector3(0, 0, 1), new Vector2(tiling, tiling)),
            new Vertex(new Vector3(1.0f, 3.0f, 1.0f), new Vector3(0, 0, 1), new Vector2(0, tiling)),

            // Left face (X-)
            new Vertex(new Vector3(-1.0f, 0.0f, 1.0f), new Vector3(-1, 0, 0), new Vector2(0, 0)),
            new Vertex(new Vector3(-1.0f, 0.0f, -1.0f), new Vector3(-1, 0, 0), new Vector2(tiling, 0)),
            new Vertex(new Vector3(-1.0f, 3.0f, -1.0f), new Vector3(-1, 0, 0), new Vector2(tiling, tiling)),
            new Vertex(new Vector3(-1.0f, 3.0f, 1.0f), new Vector3(-1, 0, 0), new Vector2(0, tiling)),

            // Right face (X+)
            new Vertex(new Vector3(1.0f, 0.0f, -1.0f), new Vector3(1, 0, 0), new Vector2(0, 0)),
            new Vertex(new Vector3(1.0f, 0.0f, 1.0f), new Vector3(1, 0, 0), new Vector2(tiling, 0)),
            new Vertex(new Vector3(1.0f, 3.0f, 1.0f), new Vector3(1, 0, 0), new Vector2(tiling, tiling)),
            new Vertex(new Vector3(1.0f, 3.0f, -1.0f), new Vector3(1, 0, 0), new Vector2(0, tiling)),
            };

            int[] indices = new int[]
            {
                0, 1, 2, 0, 2, 3,
                4, 6, 5, 4, 7, 6,
                8, 9, 10, 8, 10, 11,
                12, 13, 14, 12, 14, 15,
                16, 17, 18, 16, 18, 19,
                20, 21, 22, 20, 22, 23
            };

            Create(vertices, indices);
        }

    }
}

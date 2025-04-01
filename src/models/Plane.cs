using Game3D;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zpg.models
{
    public class Plane : Model
    {
        public Plane(Vector3 position, int sizeX, int sizeZ, float height = 0.2f) // Výška podlahy
        {
            this.position = position;

            float halfX = sizeX / 2.0f;
            float halfZ = sizeZ / 2.0f;
            float halfHeight = height / 2.0f;

            // Definujeme vrcholy pro "tlustou" podlahu
            Vertex[] vertices = new Vertex[]
            {
            // Horní vrstva
            new Vertex(new Vector3(-halfX, halfHeight, -halfZ), new Vector3(0, 1, 0)), // 0
            new Vertex(new Vector3( halfX, halfHeight, -halfZ), new Vector3(0, 1, 0)), // 1
            new Vertex(new Vector3( halfX, halfHeight,  halfZ), new Vector3(0, 1, 0)), // 2
            new Vertex(new Vector3(-halfX, halfHeight,  halfZ), new Vector3(0, 1, 0)), // 3

            // Spodní vrstva
            new Vertex(new Vector3(-halfX, -halfHeight, -halfZ), new Vector3(0, 1, 0)), // 4
            new Vertex(new Vector3( halfX, -halfHeight, -halfZ), new Vector3(0, 1, 0)), // 5
            new Vertex(new Vector3( halfX, -halfHeight,  halfZ), new Vector3(0, 1, 0)), // 6
            new Vertex(new Vector3(-halfX, -halfHeight,  halfZ), new Vector3(0, 1, 0)), // 7
            };

            // Definujeme indexy pro vykreslení kvádru pomocí trojúhelníků
            int[] indices = new int[]
            {
            // Horní strana
            0, 1, 2,  2, 3, 0,

            // Spodní strana
            4, 5, 6,  6, 7, 4,

            // Přední strana
            3, 2, 6,  6, 7, 3,

            // Zadní strana
            0, 1, 5,  5, 4, 0,

            // Levá strana
            0, 3, 7,  7, 4, 0,

            // Pravá strana
            1, 2, 6,  6, 5, 1
            };

            // Bind the model (upload data to GPU)
            Create(vertices, indices);
        }
    }
}

using Game3D;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zpg.models
{
    public class Door : Model
    {
        public bool open { get; set; }
        public int indexX;
        public int indexZ;
        public Door(Vector3 position, int indexX, int indexZ)
        {
            this.open = false;
            this.indexX = indexX;
            this.indexZ = indexZ;

            this.position = position;
            
            vertices = new Vertex[]
            {
            new Vertex(new Vector3(-1.0f, 0.0f, -1.0f)), // 0
            new Vertex(new Vector3(1.0f, 0.0f, -1.0f)),  // 1
            new Vertex(new Vector3(1.0f, 0.0f, 1.0f)),   // 2
            new Vertex(new Vector3(-1.0f, 0.0f, 1.0f)),  // 3

            new Vertex(new Vector3(-1.0f, 3.0f, -1.0f)), // 4
            new Vertex(new Vector3(1.0f, 3.0f, -1.0f)),  // 5
            new Vertex(new Vector3(1.0f, 3.0f, 1.0f)),   // 6
            new Vertex(new Vector3(-1.0f, 3.0f, 1.0f)),  // 7
            };

            indices = new int[]
            {
            0, 1, 2, 2, 3, 0,
            
            4, 5, 6, 6, 7, 4,

            0, 1, 5, 5, 4, 0,

            2, 3, 7, 7, 6, 2,

            0, 3, 7, 7, 4, 0,

            1, 2, 6, 6, 5, 1
            };

            Vertex.SimpleNormals(vertices, indices);

            this.Material = new Material(
                new Vector3(0.6f, 0.3f, 0.1f), // Ambient (hnědá barva)
                new Vector3(0.6f, 0.3f, 0.1f), // Diffuse (hnědá barva)
                new Vector3(0.2f, 0.1f, 0.05f), // Specular (nízký lesk)
                4.0f                            // Shininess (nízký lesk)
            );
            Bind();
            this.indexX = indexX;
            this.indexZ = indexZ;
        }

        public bool Check(Camera player)
        {
            if ((int)player.pos.X/2 == indexX && ((int)player.pos.Z/2 - 1 == indexZ || (int)player.pos.Z / 2 + 1 == indexZ))
            {
                player.collisions[indexZ][indexX] = false;
                return true;
            }
            else if ((int)player.pos.Z / 2 == indexZ && ((int)player.pos.X / 2 - 1 == indexX || (int)player.pos.X / 2 + 1 == indexX))
            {
                player.collisions[indexZ][indexX] = false;
                return true;
            }
            Console.WriteLine("Player X: " + (int)player.pos.X / 2 + " Z: " + (int)player.pos.Z / 2 + "; Doors X: " + indexX + "Z: " + indexZ);
            return false;
        }
    }
}

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
            
            Vertex[] vertices = new Vertex[]
            {
            new Vertex(new Vector3(-1.0f, 0.0f, -1.0f), Vector3.Zero), // 0
            new Vertex(new Vector3(1.0f, 0.0f, -1.0f), Vector3.Zero),  // 1
            new Vertex(new Vector3(1.0f, 0.0f, 1.0f), Vector3.Zero),   // 2
            new Vertex(new Vector3(-1.0f, 0.0f, 1.0f), Vector3.Zero),  // 3

            new Vertex(new Vector3(-1.0f, 3.0f, -1.0f), Vector3.Zero), // 4
            new Vertex(new Vector3(1.0f, 3.0f, -1.0f), Vector3.Zero),  // 5
            new Vertex(new Vector3(1.0f, 3.0f, 1.0f), Vector3.Zero),   // 6
            new Vertex(new Vector3(-1.0f, 3.0f, 1.0f), Vector3.Zero),  // 7
            };

            int [] indices = new int[]
            {
            0, 1, 2, 2, 3, 0,
            
            4, 5, 6, 6, 7, 4,

            0, 1, 5, 5, 4, 0,

            2, 3, 7, 7, 6, 2,

            0, 3, 7, 7, 4, 0,

            1, 2, 6, 6, 5, 1
            };

            SimpleNormals(vertices, indices);
            
            Create(vertices, indices);

            this.indexX = indexX;
            this.indexZ = indexZ;
        }

        public bool Check(Camera player, bool[][] collisions)
        {
            if ((int)player.position.X/2 == indexX && ((int)player.position.Z/2 - 1 == indexZ || (int)player.position.Z / 2 + 1 == indexZ))
            {
                collisions[indexZ][indexX] = false;
                return true;
            }
            else if ((int)player.position.Z / 2 == indexZ && ((int)player.position.X / 2 - 1 == indexX || (int)player.position.X / 2 + 1 == indexX))
            {
                collisions[indexZ][indexX] = false;
                return true;
            }
            Console.WriteLine("Player X: " + (int)player.position.X / 2 + " Z: " + (int)player.position.Z / 2 + "; Doors X: " + indexX + "Z: " + indexZ);
            return false;
        }
    }
}

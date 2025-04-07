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
                new Vertex(new Vector3(-1.0f, 0.0f, -1.0f), new Vector3(0, -1, 0)),
                new Vertex(new Vector3(1.0f, 0.0f, -1.0f), new Vector3(0, -1, 0)),
                new Vertex(new Vector3(1.0f, 0.0f, 1.0f), new Vector3(0, -1, 0)),
                new Vertex(new Vector3(-1.0f, 0.0f, 1.0f), new Vector3(0, -1, 0)),
        
                new Vertex(new Vector3(-1.0f, 3.0f, -1.0f), new Vector3(0, 1, 0)),
                new Vertex(new Vector3(1.0f, 3.0f, -1.0f), new Vector3(0, 1, 0)),
                new Vertex(new Vector3(1.0f, 3.0f, 1.0f), new Vector3(0, 1, 0)),
                new Vertex(new Vector3(-1.0f, 3.0f, 1.0f), new Vector3(0, 1, 0)),
        
                new Vertex(new Vector3(-1.0f, 0.0f, -1.0f), new Vector3(0, 0, -1)),
                new Vertex(new Vector3(1.0f, 0.0f, -1.0f), new Vector3(0, 0, -1)),
                new Vertex(new Vector3(1.0f, 3.0f, -1.0f), new Vector3(0, 0, -1)),
                new Vertex(new Vector3(-1.0f, 3.0f, -1.0f), new Vector3(0, 0, -1)),
        
                new Vertex(new Vector3(1.0f, 0.0f, 1.0f), new Vector3(0, 0, 1)),
                new Vertex(new Vector3(-1.0f, 0.0f, 1.0f), new Vector3(0, 0, 1)),
                new Vertex(new Vector3(-1.0f, 3.0f, 1.0f), new Vector3(0, 0, 1)),
                new Vertex(new Vector3(1.0f, 3.0f, 1.0f), new Vector3(0, 0, 1)),
        
                new Vertex(new Vector3(-1.0f, 0.0f, 1.0f), new Vector3(-1, 0, 0)),
                new Vertex(new Vector3(-1.0f, 0.0f, -1.0f), new Vector3(-1, 0, 0)),
                new Vertex(new Vector3(-1.0f, 3.0f, -1.0f), new Vector3(-1, 0, 0)),
                new Vertex(new Vector3(-1.0f, 3.0f, 1.0f), new Vector3(-1, 0, 0)),
        
                new Vertex(new Vector3(1.0f, 0.0f, -1.0f), new Vector3(1, 0, 0)),
                new Vertex(new Vector3(1.0f, 0.0f, 1.0f), new Vector3(1, 0, 0)),
                new Vertex(new Vector3(1.0f, 3.0f, 1.0f), new Vector3(1, 0, 0)),
                new Vertex(new Vector3(1.0f, 3.0f, -1.0f), new Vector3(1, 0, 0)),
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
            this.indexX = indexX;
            this.indexZ = indexZ;
        }

        public bool Check(Camera player, bool[][] collisions)
        {
            if ((int)player.pos.X/2 == indexX && ((int)player.pos.Z/2 - 1 == indexZ || (int)player.pos.Z / 2 + 1 == indexZ))
            {
                collisions[indexZ][indexX] = false;
                return true;
            }
            else if ((int)player.pos.Z / 2 == indexZ && ((int)player.pos.X / 2 - 1 == indexX || (int)player.pos.X / 2 + 1 == indexX))
            {
                collisions[indexZ][indexX] = false;
                return true;
            }
            Console.WriteLine("Player X: " + (int)player.pos.X / 2 + " Z: " + (int)player.pos.Z / 2 + "; Doors X: " + indexX + "Z: " + indexZ);
            return false;
        }
    }
}

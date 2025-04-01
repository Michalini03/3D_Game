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
            // Definování vrcholů sloupu (2x2 základna, výška 3)
            Vertex[] vertices = new Vertex[]
            {
            // Dolní čtverec (y = 0)
            new Vertex(new Vector3(-1.0f, 0.0f, -1.0f), Vector3.Zero), // 0
            new Vertex(new Vector3(1.0f, 0.0f, -1.0f), Vector3.Zero),  // 1
            new Vertex(new Vector3(1.0f, 0.0f, 1.0f), Vector3.Zero),   // 2
            new Vertex(new Vector3(-1.0f, 0.0f, 1.0f), Vector3.Zero),  // 3

            // Horní čtverec (y = 3)
            new Vertex(new Vector3(-1.0f, 3.0f, -1.0f), Vector3.Zero), // 4
            new Vertex(new Vector3(1.0f, 3.0f, -1.0f), Vector3.Zero),  // 5
            new Vertex(new Vector3(1.0f, 3.0f, 1.0f), Vector3.Zero),   // 6
            new Vertex(new Vector3(-1.0f, 3.0f, 1.0f), Vector3.Zero),  // 7
            };

            // Indexy pro kreslení trojúhelníků (6 stěn, 2 trojúhelníky na každé)
            int[] indices = new int[]
            {
            // Spodní stěna
            0, 1, 2, 0, 2, 3,
            
            // Horní stěna
            4, 5, 6, 6, 7, 4,

            // Přední stěna
            0, 1, 5, 5, 4, 0,

            // Zadní stěna
            2, 3, 7, 7, 6, 2,

            // Levá stěna
            0, 3, 7, 7, 4, 0,

            // Pravá stěna
            1, 2, 6, 6, 5, 1
            };

            SimpleNormals(vertices, indices);
            Create(vertices, indices);
        }
    }
}

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
            vertices = new Vertex[]
            {
            // Dolní čtverec (y = 0)
            new Vertex(new Vector3(-1.0f, 0.0f, -1.0f)), // 0
            new Vertex(new Vector3(1.0f, 0.0f, -1.0f)),  // 1
            new Vertex(new Vector3(1.0f, 0.0f, 1.0f)),   // 2
            new Vertex(new Vector3(-1.0f, 0.0f, 1.0f)),  // 3

            // Horní čtverec (y = 3)
            new Vertex(new Vector3(-1.0f, 3.0f, -1.0f)), // 4
            new Vertex(new Vector3(1.0f, 3.0f, -1.0f)),  // 5
            new Vertex(new Vector3(1.0f, 3.0f, 1.0f)),   // 6
            new Vertex(new Vector3(-1.0f, 3.0f, 1.0f)),  // 7
            };

            // Indexy pro kreslení trojúhelníků (6 stěn, 2 trojúhelníky na každé)
            indices = new int[]
            {
            // Spodní stěna
            0, 1, 2, 2, 3, 0,
            
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

            Vertex.SimpleNormals(vertices, indices);

            this.Material = new Material(
                new Vector3(0.2f, 0.2f, 0.2f), // Ambient (šedá barva)
                new Vector3(1.0f, 0.0f, 0.0f), // Diffuse (červená barva)
                new Vector3(1.0f, 0.0f, 0.0f), // Specular (červená barva)
                32.0f                          // Shininess (lesk)
            );
            Bind();
        }
    }
}

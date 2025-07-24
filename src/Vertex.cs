using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Game3D
{
    /// <summary>
    /// Struktura pro vrchol 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 64)]
    public struct Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 TexCoords;

        public Vertex(Vector3 position, Vector3 normal, Vector2 texCoords)
        {
            this.position = position;
            this.normal = normal;
            this.TexCoords = texCoords;
        }

        public Vertex(Vector3 position, Vector3 normal)
        {
            this.position = position;
            this.normal = normal;
            this.TexCoords = Vector2.Zero;
        }

        
        /// <summary>
        /// Vrátí velikost struktury v bytech
        /// </summary>
        /// <returns></returns>
        public static int SizeOf()
        {
            return Marshal.SizeOf(typeof(Vertex));
        }
    }
}

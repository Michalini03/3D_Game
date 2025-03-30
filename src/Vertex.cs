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

        public Vertex(Vector3 position)
        {
            this.position = position;
            this.normal = Vector3.Zero;
        }

        public static void SimpleNormals(Vertex[] vertices, int[] indices)
        {
            for (int i = 0; i < indices.Length; i += 3)
            {
                int index1 = indices[i];
                int index2 = indices[i + 1];
                int index3 = indices[i + 2];

                Vector3 vertex1 = vertices[index1].position;
                Vector3 vertex2 = vertices[index2].position;
                Vector3 vertex3 = vertices[index3].position;

                Vector3 edge1 = vertex2 - vertex1;
                Vector3 edge2 = vertex3 - vertex1;

                Vector3 triangleNormal = Vector3.Cross(edge1, edge2);

                triangleNormal.Normalize();

                vertices[index1].normal += triangleNormal;
                vertices[index2].normal += triangleNormal;
                vertices[index3].normal += triangleNormal;


            }

            for(int i = 0; i < vertices.Length; i++)
            {
                vertices[i].normal.Normalize();
            }
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

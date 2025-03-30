using Game3D;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Zpg;

namespace Zpg.models
{
    public class SimpleObj : Model
    {
        public SimpleObj(string filename, Vector3 position)
        {
            this.position = position;

            string[] lines = File.ReadAllLines(filename);
            int cnt = 0;
            for (cnt = 0; cnt < lines.Length; cnt++)
            {
                if (lines[cnt][0] != 'v') break;
            }
            vertices = new Vertex[cnt];
            for (int i = 0; i < cnt; i++)
            {
                var tokens = lines[i].Split(' ');
                float.TryParse(tokens[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float x);
                float.TryParse(tokens[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float y);
                float.TryParse(tokens[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float z);
                vertices[i] = new Vertex(new Vector3(x, y, z));
            }
            indices = new int[3 * (lines.Length - cnt)];
            int k = 0;
            for (int i = cnt; i < lines.Length; i++)
            {
                var tokens = lines[i].Split(' ');
                int.TryParse(tokens[1].Split("/")[0], out int i1);
                int.TryParse(tokens[2].Split("/")[0], out int i2);
                int.TryParse(tokens[3].Split("/")[0], out int i3);
                indices[k + 0] = i1 - 1;
                indices[k + 1] = i2 - 1;
                indices[k + 2] = i3 - 1;
                k += 3;
            }

            // Compute normals
            Vertex.SimpleNormals(vertices, indices);

            Material = new Material(
                new Vector3(0.2f, 0.2f, 0.2f), // ambient
                new Vector3(0.2f, 0.2f, 0.2f), // diffuse
                new Vector3(0.8f, 0.8f, 0.8f), // specular
                32.0f
            );
            Bind();
        }
    }
}

using Game3D;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Zpg.models
{
    public class BetterObj : Model
    {
        public BetterObj(string filename, Vector3 position)
        {
            this.position = position;

            List<Vector3> positions = new();
            List<Vector2> texcoords = new();
            List<Vector3> normals = new();

            List<Vertex> vertices = new();
            List<int> indices = new();
            Dictionary<string, int> uniqueVertexMap = new();

            string[] lines = File.ReadAllLines(filename);

            foreach (var line in lines)
            {
                if (line.StartsWith("v "))
                {
                    var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    positions.Add(new Vector3(
                        float.Parse(tokens[1], CultureInfo.InvariantCulture),
                        float.Parse(tokens[2], CultureInfo.InvariantCulture),
                        float.Parse(tokens[3], CultureInfo.InvariantCulture)));
                }
                else if (line.StartsWith("vt "))
                {
                    var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    texcoords.Add(new Vector2(
                        float.Parse(tokens[1], CultureInfo.InvariantCulture),
                        float.Parse(tokens[2], CultureInfo.InvariantCulture)));
                }
                else if (line.StartsWith("vn "))
                {
                    var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    normals.Add(new Vector3(
                        float.Parse(tokens[1], CultureInfo.InvariantCulture),
                        float.Parse(tokens[2], CultureInfo.InvariantCulture),
                        float.Parse(tokens[3], CultureInfo.InvariantCulture)));
                }
                else if (line.StartsWith("f "))
                {
                    var tokens = line.Substring(2).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    int[] faceIndices = new int[tokens.Length];

                    for (int i = 0; i < tokens.Length; i++)
                    {
                        var token = tokens[i];

                        if (!uniqueVertexMap.TryGetValue(token, out int index))
                        {
                            var parts = token.Split('/');
                            int vi = int.Parse(parts[0]) - 1;
                            int ti = parts.Length > 1 && parts[1] != "" ? int.Parse(parts[1]) - 1 : -1;
                            int ni = parts.Length > 2 ? int.Parse(parts[2]) - 1 : -1;

                            Vector3 pos = positions[vi];
                            Vector2 tex = ti >= 0 && ti < texcoords.Count ? texcoords[ti] : Vector2.Zero;
                            Vector3 norm = ni >= 0 && ni < normals.Count ? normals[ni] : Vector3.Zero;

                            index = vertices.Count;
                            uniqueVertexMap[token] = index;
                            vertices.Add(new Vertex(pos, norm, tex));
                        }

                        faceIndices[i] = index;
                    }

                    // Triangulate the polygon (fan method)
                    for (int i = 1; i < faceIndices.Length - 1; i++)
                    {
                        indices.Add(faceIndices[0]);
                        indices.Add(faceIndices[i]);
                        indices.Add(faceIndices[i + 1]);
                    }
                }
            }

            Create(vertices.ToArray(), indices.ToArray());
        }
    }
}

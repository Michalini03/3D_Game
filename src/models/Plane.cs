using Game3D;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Zpg.models
{
    public class Plane : Model
    {
        public Plane(Vector3 position, int sizeX, int sizeZ, int holeX = -1, int holeZ = -1)
        {
            this.position = position;

            float halfHeight = 0.05f; // Half thickness of the floor (total = 0.1f)

            List<Vertex> vertices = new List<Vertex>();
            List<int> indices = new List<int>();

            int indexOffset = 0;

            int indexX = -1;
            int indexZ = -1;

            for (int x = (-sizeX / 2) + 1; x < (sizeX / 2); x += 2)
            {
                indexX++;
                indexZ = -1;
                for (int z = (-sizeZ / 2) + 1; z < sizeZ / 2; z += 2)
                {
                    indexZ++;
                    float worldX = x;
                    float worldZ = z;

                    bool isHole = (indexX == holeX && indexZ == holeZ);
                    if (isHole) continue;

                    Vector3 topNormal = new Vector3(0, 1, 0);
                    Vector3 bottomNormal = new Vector3(0, -1, 0);

                    // Top face corners
                    Vector3 topBL = new Vector3(worldX - 1f, halfHeight, worldZ - 1f);
                    Vector3 topBR = new Vector3(worldX + 1f, halfHeight, worldZ - 1f);
                    Vector3 topTR = new Vector3(worldX + 1f, halfHeight, worldZ + 1f);
                    Vector3 topTL = new Vector3(worldX - 1f, halfHeight, worldZ + 1f);

                    // Bottom face corners
                    Vector3 bottomBL = new Vector3(worldX - 1f, -halfHeight, worldZ - 1f);
                    Vector3 bottomBR = new Vector3(worldX + 1f, -halfHeight, worldZ - 1f);
                    Vector3 bottomTR = new Vector3(worldX + 1f, -halfHeight, worldZ + 1f);
                    Vector3 bottomTL = new Vector3(worldX - 1f, -halfHeight, worldZ + 1f);

                    // UVs
                    Vector2 uvBL = new Vector2(0, 0);
                    Vector2 uvBR = new Vector2(1, 0);
                    Vector2 uvTR = new Vector2(1, 1);
                    Vector2 uvTL = new Vector2(0, 1);

                    // Add top face vertices
                    vertices.Add(new Vertex(topBL, topNormal, uvBL));
                    vertices.Add(new Vertex(topBR, topNormal, uvBR));
                    vertices.Add(new Vertex(topTR, topNormal, uvTR));
                    vertices.Add(new Vertex(topTL, topNormal, uvTL));

                    // Add bottom face vertices
                    vertices.Add(new Vertex(bottomBL, bottomNormal, uvBL));
                    vertices.Add(new Vertex(bottomBR, bottomNormal, uvBR));
                    vertices.Add(new Vertex(bottomTR, bottomNormal, uvTR));
                    vertices.Add(new Vertex(bottomTL, bottomNormal, uvTL));

                    // Indices for top face
                    indices.Add(indexOffset + 0);
                    indices.Add(indexOffset + 1);
                    indices.Add(indexOffset + 2);
                    indices.Add(indexOffset + 0);
                    indices.Add(indexOffset + 2);
                    indices.Add(indexOffset + 3);

                    // Indices for bottom face
                    indices.Add(indexOffset + 4);
                    indices.Add(indexOffset + 6);
                    indices.Add(indexOffset + 5);
                    indices.Add(indexOffset + 4);
                    indices.Add(indexOffset + 7);
                    indices.Add(indexOffset + 6);

                    indexOffset += 8;
                }
            }

            Create(vertices.ToArray(), indices.ToArray());
        }
    }
}

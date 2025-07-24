using Game3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Zpg.models
{
    class Skybox : Model
    {
        public Skybox(Vector3 position, float size = 25)
        {
            // Don't subtract from the half size, this could be making your skybox too small
            float halfSize = size / 2.0f;

            float highest = 15f;

            float mapSizeX = position.X + highest; 
            float mapSizeZ = position.Z + highest;

            // Position the skybox at camera position (typically for skyboxes)
            // Note: In most engines, the skybox should follow the camera position
            this.position = position;

            // 6 faces * 4 vertices per face = 24 vertices
            Vertex[] vertices = new Vertex[]
            {
                // Front face (+Z)
                new Vertex(new Vector3(-mapSizeX,  highest,  mapSizeZ), new Vector3(0, 0, -1), new Vector2(0.25f, 0.667f)),
                new Vertex(new Vector3( mapSizeX,  highest,  mapSizeZ), new Vector3(0, 0, -1), new Vector2(0.5f, 0.667f)),
                new Vertex(new Vector3( mapSizeX, -highest,  mapSizeZ), new Vector3(0, 0, -1), new Vector2(0.5f, 0.334f)),
                new Vertex(new Vector3(-mapSizeX, -highest,  mapSizeZ), new Vector3(0, 0, -1), new Vector2(0.25f, 0.334f)),

                // Back face (-Z)
                new Vertex(new Vector3( mapSizeX,  highest, -mapSizeZ), new Vector3(0, 0, 1), new Vector2(0.75f, 0.667f)),
                new Vertex(new Vector3(-mapSizeX,  highest, -mapSizeZ), new Vector3(0, 0, 1), new Vector2(1.0f, 0.667f)),
                new Vertex(new Vector3(-mapSizeX, -highest, -mapSizeZ), new Vector3(0, 0, 1), new Vector2(1.0f, 0.334f)),
                new Vertex(new Vector3( mapSizeX, -highest, -mapSizeZ), new Vector3(0, 0, 1), new Vector2(0.75f, 0.334f)),

                // Top face (+Y)
                new Vertex(new Vector3(-mapSizeX,  highest, -mapSizeZ), new Vector3(0, -1, 0), new Vector2(0.25f, 1.0f)),
                new Vertex(new Vector3( mapSizeX,  highest, -mapSizeZ), new Vector3(0, -1, 0), new Vector2(0.5f, 1.0f)),
                new Vertex(new Vector3( mapSizeX,  highest,  mapSizeZ), new Vector3(0, -1, 0), new Vector2(0.5f, 0.667f)),
                new Vertex(new Vector3(-mapSizeX,  highest,  mapSizeZ), new Vector3(0, -1, 0), new Vector2(0.25f, 0.667f)),

                // Bottom face (-Y)
                new Vertex(new Vector3(-mapSizeX, -highest,  mapSizeZ), new Vector3(0, 1, 0), new Vector2(0.25f, 0.334f)),
                new Vertex(new Vector3( mapSizeX, -highest,  mapSizeZ), new Vector3(0, 1, 0), new Vector2(0.5f, 0.334f)),
                new Vertex(new Vector3( mapSizeX, -highest, -mapSizeZ), new Vector3(0, 1, 0), new Vector2(0.5f, 0.0f)),
                new Vertex(new Vector3(-mapSizeX, -highest, -mapSizeZ), new Vector3(0, 1, 0), new Vector2(0.25f, 0.0f)),

                // Right face (+X)
                new Vertex(new Vector3( mapSizeX,  highest,  mapSizeZ), new Vector3(-1, 0, 0), new Vector2(0.5f, 0.667f)),
                new Vertex(new Vector3( mapSizeX,  highest, -mapSizeZ), new Vector3(-1, 0, 0), new Vector2(0.75f, 0.667f)),
                new Vertex(new Vector3( mapSizeX, -highest, -mapSizeZ), new Vector3(-1, 0, 0), new Vector2(0.75f, 0.334f)),
                new Vertex(new Vector3( mapSizeX, -highest,  mapSizeZ), new Vector3(-1, 0, 0), new Vector2(0.5f, 0.334f)),

                // Left face (-X)
                new Vertex(new Vector3(-mapSizeX,  highest, -mapSizeZ), new Vector3(1, 0, 0), new Vector2(0.0f, 0.667f)),
                new Vertex(new Vector3(-mapSizeX,  highest,  mapSizeZ), new Vector3(1, 0, 0), new Vector2(0.25f, 0.667f)),
                new Vertex(new Vector3(-mapSizeX, -highest,  mapSizeZ), new Vector3(1, 0, 0), new Vector2(0.25f, 0.334f)),
                new Vertex(new Vector3(-mapSizeX, -highest, -mapSizeZ), new Vector3(1, 0, 0), new Vector2(0.0f, 0.334f)),
            };

            // Keep indices as they were
            int[] indices = new int[]
            {
                0, 1, 2,   2, 3, 0,       // Front
                4, 5, 6,   6, 7, 4,       // Back
                8, 9,10,  10,11, 8,       // Top
               12,13,14,  14,15,12,       // Bottom
               16,17,18,  18,19,16,       // Right
               20,21,22,  22,23,20        // Left
            };

            // Make sure mesh is properly created and stored
            Create(vertices, indices);
        }
    } 
}

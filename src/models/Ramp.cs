using Game3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;

namespace Zpg.models
{
    public class Ramp : Model
    {
        private int floor;
        public Ramp(Vector3 position, int k)
        {
            this.position = position;
            this.floor = k;
            float tiling = 2.0f;   // How much the texture repeats
            float height = 3.0f;

            // Calculate proper normal for the sloped ramp surface
            Vector3 rampNormal = Vector3.Normalize(Vector3.Cross(
                new Vector3(2.0f, 0.0f, 0.0f),
                new Vector3(0.0f, height, 2.0f)
            ));

            Vertex[] vertices = new Vertex[]
            {
                // Bottom face vertices (indices 0-3)
                new Vertex(new Vector3(-1.0f, 0.0f, -1.0f), new Vector3(0, -1, 0), new Vector2(0, 0)),      // 0
                new Vertex(new Vector3( 1.0f, 0.0f, -1.0f), new Vector3(0, -1, 0), new Vector2(tiling, 0)), // 1
                new Vertex(new Vector3( 1.0f, 0.0f,  1.0f), new Vector3(0, -1, 0), new Vector2(tiling, tiling)), // 2
                new Vertex(new Vector3(-1.0f, 0.0f,  1.0f), new Vector3(0, -1, 0), new Vector2(0, tiling)), // 3
                
                // Ramp surface vertices (indices 4-7)
                new Vertex(new Vector3(-1.0f, 0.0f, -1.0f), rampNormal, new Vector2(0, 0)),        // 4
                new Vertex(new Vector3( 1.0f, 0.0f, -1.0f), rampNormal, new Vector2(tiling, 0)),   // 5
                new Vertex(new Vector3( 1.0f, height,  1.0f), rampNormal, new Vector2(tiling, tiling)), // 6
                new Vertex(new Vector3(-1.0f, height,  1.0f), rampNormal, new Vector2(0, tiling)),  // 7
                
                // Back face vertices (high side) (indices 8-11)
                new Vertex(new Vector3(-1.0f, 0.0f,  1.0f), new Vector3(0, 0, 1), new Vector2(0, 0)),      // 8
                new Vertex(new Vector3( 1.0f, 0.0f,  1.0f), new Vector3(0, 0, 1), new Vector2(tiling, 0)), // 9
                new Vertex(new Vector3( 1.0f, height, 1.0f), new Vector3(0, 0, 1), new Vector2(tiling, tiling)), // 10
                new Vertex(new Vector3(-1.0f, height, 1.0f), new Vector3(0, 0, 1), new Vector2(0, tiling)), // 11
                
                // Front face vertices (low side) (indices 12-13)
                new Vertex(new Vector3(-1.0f, 0.0f, -1.0f), new Vector3(0, 0, -1), new Vector2(0, 0)),     // 12
                new Vertex(new Vector3( 1.0f, 0.0f, -1.0f), new Vector3(0, 0, -1), new Vector2(tiling, 0)), // 13
                
                // Left face vertices (indices 14-16)
                new Vertex(new Vector3(-1.0f, 0.0f, -1.0f), new Vector3(-1, 0, 0), new Vector2(0, 0)),     // 14
                new Vertex(new Vector3(-1.0f, 0.0f,  1.0f), new Vector3(-1, 0, 0), new Vector2(tiling, 0)), // 15
                new Vertex(new Vector3(-1.0f, height, 1.0f), new Vector3(-1, 0, 0), new Vector2(tiling, height/2.0f)), // 16
                
                // Right face vertices (indices 17-19)
                new Vertex(new Vector3(1.0f, 0.0f, -1.0f), new Vector3(1, 0, 0), new Vector2(0, 0)),      // 17
                new Vertex(new Vector3(1.0f, 0.0f,  1.0f), new Vector3(1, 0, 0), new Vector2(tiling, 0)), // 18
                new Vertex(new Vector3(1.0f, height, 1.0f), new Vector3(1, 0, 0), new Vector2(tiling, height/2.0f)), // 19
            };

            int[] indices = new int[]
            {
                0, 2, 1, 0, 3, 2,
                
                4, 6, 5, 4, 7, 6,
                
                8, 10, 9, 8, 11, 10,
                
                12, 13, 1, 12, 1, 0,
                
                14, 16, 15, 14, 16, 15,
                
                17, 18, 19, 17, 19, 18,
            };

            Create(vertices, indices);
        }

        public void isOnRamp(Camera player, Vector3 previousPosition)
        {
            float playerX = player.pos.X;
            float playerZ = player.pos.Z;

            float playerY = player.pos.Y;

            float rampX = position.X;
            float rampZ = position.Z;

            if (playerX >= rampX - 1.15f && playerX <= rampX + 1.15f &&
                playerZ >= rampZ - 1.15f && playerZ <= rampZ + 1.15f)
            {
                bool comingFromSide = (previousPosition.X <= rampX - 1.15f || previousPosition.X >= rampX + 1.15f);

                
                if (comingFromSide)
                {
                    // Block side approaches
                    Console.WriteLine("Player is coming from side - blocked");
                    player.pos = new Vector3(previousPosition.X, previousPosition.Y, playerZ);
                    return;
                }

                const float rampThreshold = 1.15f;

                if (previousPosition.Z <= rampZ - rampThreshold && player.k != this.floor - 1)
                {
                    Console.WriteLine("Player is coming from front - blocked");
                    player.pos = new Vector3(playerX, previousPosition.Y, previousPosition.Z);
                    return;
                }
                if (previousPosition.Z >= rampZ + rampThreshold && player.k != this.floor)
                {
                    Console.WriteLine("Player is coming from back - blocked");
                    player.pos = new Vector3(playerX, previousPosition.Y, previousPosition.Z);
                    return;
                }

                // <-1, 1>
                float progress = playerZ - rampZ;
                float rampHeight = 3.1f; // Max height of the ramp
                float originalPlayerY = 1.7f;

                if (progress >= 0.98f) player.k = this.floor;
                else if (progress <= -0.98f) player.k = this.floor - 1;
                float normalizedProgress = (progress + 1) / 2;
                playerY = originalPlayerY + normalizedProgress * rampHeight + (rampHeight * (this.floor - 1));
                player.pos = new Vector3(playerX, playerY, playerZ);
            }
        }

    }
}
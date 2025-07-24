using Game3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using System.Buffers.Text;

namespace Zpg.models
{
    class Teleport : Model
    {
        public int key;
        public float elapsedTime = 0.0f;
        public float baseY;

        public Teleport pair;

        // Teleportation state
        private bool isTeleporting = false;
        private float teleportTimer = 0.0f;
        private const float TELEPORT_DELAY = 5.0f;
        private Vector3 teleportStartPosition;
        private const float TELEPORT_RADIUS = 0.8f; // Slightly larger than the diamond (0.6f)

        public Teleport(Vector3 position, int key)
        {
            this.position = position;
            this.baseY = position.Y;
            this.key = key;

            Vertex[] teleporterVertices = new Vertex[]
            {
                new Vertex(new Vector3(0.0f, 0.75f, 0.0f), new Vector3(0, -1, 0), new Vector2(0.5f, 0.5f)),
    
                new Vertex(new Vector3(0.6f, 1.5f, 0.0f), new Vector3(1, 0, 0), new Vector2(1, 0.5f)),     // Right
                new Vertex(new Vector3(0.0f, 1.5f, 0.6f), new Vector3(0, 0, 1), new Vector2(0.5f, 1)),     // Front
                new Vertex(new Vector3(-0.6f, 1.5f, 0.0f), new Vector3(-1, 0, 0), new Vector2(0, 0.5f)),   // Left
                new Vertex(new Vector3(0.0f, 1.5f, -0.6f), new Vector3(0, 0, -1), new Vector2(0.5f, 0)),   // Back
    
                new Vertex(new Vector3(0.0f, 2.25f, 0.0f), new Vector3(0, 1, 0), new Vector2(0.5f, 0.5f))
            };

            int[] teleporterIndices = new int[]
            {
                0, 2, 1,    // Bottom to Front to Right
                0, 3, 2,    // Bottom to Left to Front
                0, 4, 3,    // Bottom to Back to Left
                0, 1, 4,    // Bottom to Right to Back
    
                1, 2, 5,    // Right to Front to Top
                2, 3, 5,    // Front to Left to Top
                3, 4, 5,    // Left to Back to Top
                4, 1, 5,    // Back to Right to Top
    
                1, 3, 2,    // Right to Left to Front (visible from outside)
                1, 4, 3     // Right to Back to Left (visible from outside)
            };

            Create(teleporterVertices, teleporterIndices);
        }

        public void Animation(float deltaTime)
        {
            elapsedTime += deltaTime;

            // Bobbing up and down (sin wave)
            Vector3 newPosition = position;
            newPosition.Y = baseY + (float)Math.Sin(elapsedTime * 2f) * 0.1f; // baseY is the starting Y position
            position = newPosition;

            // Continuous rotation
            rotationAngle += deltaTime * 1.23f; // Rotate faster by increasing multiplier
            if (rotationAngle > MathHelper.TwoPi)
                rotationAngle -= MathHelper.TwoPi;
        }

        public bool Check(Camera player)
        {
            // Calculate horizontal distance (ignore Y difference for teleporter entrance)
            float deltaX = player.pos.X - position.X;
            float deltaZ = player.pos.Z - position.Z;
            float horizontalDistance = (float)Math.Sqrt(deltaX * deltaX + deltaZ * deltaZ);

            // Check if player is within the teleporter area and at appropriate height
            bool isInRange = horizontalDistance <= TELEPORT_RADIUS;
            bool isAtCorrectHeight = player.pos.Y >= position.Y + 0.5f && player.pos.Y <= position.Y + 2.5f;

            return isInRange && isAtCorrectHeight;
        }

        public void Teleporting(Camera player, float deltaTime)
        {
            bool playerInside = Check(player);

            if (playerInside && !isTeleporting)
            {
                // Player just entered teleporter - start teleporting process
                isTeleporting = true;
                teleportTimer = 0.0f;
                teleportStartPosition = player.pos;
                Console.WriteLine($"Teleportation started! Please wait {TELEPORT_DELAY} seconds...");
            }
            else if (isTeleporting)
            {
                // Currently teleporting
                teleportTimer += deltaTime;

                // Keep player locked in position during teleportation
                if (playerInside)
                {
                    // Optionally keep player at center of teleporter
                    Vector3 lockedPosition = new Vector3(position.X, player.pos.Y, position.Z);
                }
                else
                {
                    // Player moved outside - cancel teleportation
                    isTeleporting = false;
                    teleportTimer = 0.0f;
                    Console.WriteLine("Teleportation cancelled - player moved outside teleporter area!");
                    return;
                }

                // Check if teleportation time is complete
                if (teleportTimer >= TELEPORT_DELAY)
                {
                    // Perform teleportation
                    if (pair != null)
                    {
                        player.pos = new Vector3(pair.position.X, player.pos.Y, pair.position.Z);
                        Console.WriteLine($"Teleported to teleporter {pair.position}!");
                    }
                    else
                    {
                        Console.WriteLine("Warning: No teleporter pair found!");
                    }

                    // Reset teleportation state
                    isTeleporting = false;
                    teleportTimer = 0.0f;
                }
                else
                {
                    // Show countdown (optional)
                    float remainingTime = TELEPORT_DELAY - teleportTimer;
                    if ((int)remainingTime != (int)(remainingTime + deltaTime))
                    {
                        Console.WriteLine($"Teleporting in {(int)Math.Ceiling(remainingTime)} seconds...");
                    }
                }
            }
        }

        public float GetWhiteFadeIntensity()
        {
            if (!isTeleporting) return 0.0f;

            float progress = TeleportProgress;
            return progress * progress;
        }

        // Getter for teleportation status (useful for UI or other systems)
        public bool IsTeleporting => isTeleporting;
        public float TeleportProgress => isTeleporting ? teleportTimer / TELEPORT_DELAY : 0.0f;
    }
}


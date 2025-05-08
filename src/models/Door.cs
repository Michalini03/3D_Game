using Game3D;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zpg.models
{
    public class Door : Model
    {
        public bool open { get; set; }
        public int indexX;
        public int indexZ;

        // Animation properties
        private bool isAnimating;
        private Vector3 closedPosition;
        private Vector3 openPosition;
        private float animationProgress;
        private float slideSpeed = 2.0f; // Units per second
        private DoorOrientation orientation;

        // Define door orientation enum
        public enum DoorOrientation
        {
            NorthSouth, // Z-axis door (slides north/south)
            EastWest    // X-axis door (slides east/west)
        }

        public Door(Vector3 position, int indexX, int indexZ, bool[][] collisions)
        {
            this.open = false;
            this.indexX = indexX;
            this.indexZ = indexZ;
            this.position = position;
            this.closedPosition = position;
            this.isAnimating = false;
            this.animationProgress = 0.0f;

            // Determine door orientation based on position in the map
            DetermineOrientation(collisions);

            // Calculate where the door should slide to when opened
            CalculateOpenPosition();

            Vertex[] vertices = new Vertex[]
            {
                new Vertex(new Vector3(-1.0f, 0.0f, -1.0f), new Vector3(0, -1, 0)),
                new Vertex(new Vector3(1.0f, 0.0f, -1.0f), new Vector3(0, -1, 0)),
                new Vertex(new Vector3(1.0f, 0.0f, 1.0f), new Vector3(0, -1, 0)),
                new Vertex(new Vector3(-1.0f, 0.0f, 1.0f), new Vector3(0, -1, 0)),

                new Vertex(new Vector3(-1.0f, 3.0f, -1.0f), new Vector3(0, 1, 0)),
                new Vertex(new Vector3(1.0f, 3.0f, -1.0f), new Vector3(0, 1, 0)),
                new Vertex(new Vector3(1.0f, 3.0f, 1.0f), new Vector3(0, 1, 0)),
                new Vertex(new Vector3(-1.0f, 3.0f, 1.0f), new Vector3(0, 1, 0)),

                new Vertex(new Vector3(-1.0f, 0.0f, -1.0f), new Vector3(0, 0, -1)),
                new Vertex(new Vector3(1.0f, 0.0f, -1.0f), new Vector3(0, 0, -1)),
                new Vertex(new Vector3(1.0f, 3.0f, -1.0f), new Vector3(0, 0, -1)),
                new Vertex(new Vector3(-1.0f, 3.0f, -1.0f), new Vector3(0, 0, -1)),

                new Vertex(new Vector3(1.0f, 0.0f, 1.0f), new Vector3(0, 0, 1)),
                new Vertex(new Vector3(-1.0f, 0.0f, 1.0f), new Vector3(0, 0, 1)),
                new Vertex(new Vector3(-1.0f, 3.0f, 1.0f), new Vector3(0, 0, 1)),
                new Vertex(new Vector3(1.0f, 3.0f, 1.0f), new Vector3(0, 0, 1)),

                new Vertex(new Vector3(-1.0f, 0.0f, 1.0f), new Vector3(-1, 0, 0)),
                new Vertex(new Vector3(-1.0f, 0.0f, -1.0f), new Vector3(-1, 0, 0)),
                new Vertex(new Vector3(-1.0f, 3.0f, -1.0f), new Vector3(-1, 0, 0)),
                new Vertex(new Vector3(-1.0f, 3.0f, 1.0f), new Vector3(-1, 0, 0)),

                new Vertex(new Vector3(1.0f, 0.0f, -1.0f), new Vector3(1, 0, 0)),
                new Vertex(new Vector3(1.0f, 0.0f, 1.0f), new Vector3(1, 0, 0)),
                new Vertex(new Vector3(1.0f, 3.0f, 1.0f), new Vector3(1, 0, 0)),
                new Vertex(new Vector3(1.0f, 3.0f, -1.0f), new Vector3(1, 0, 0)),
            };

            int[] indices = new int[]
            {
                0, 1, 2, 0, 2, 3,

                4, 6, 5, 4, 7, 6,

                8, 9, 10, 8, 10, 11,

                12, 13, 14, 12, 14, 15,

                16, 17, 18, 16, 18, 19,

                20, 21, 22, 20, 22, 23
            };

            Create(vertices, indices);
        }

        // Determine the orientation of the door based on its position in the map
        private void DetermineOrientation(bool[][] collision)
        {
            // For simplicity, we'll use the door's position to determine orientation
            // If the door's X coordinate is odd, it's an east-west door
            // If the door's Z coordinate is odd, it's a north-south door
            int x = (int)position.X / 2;
            int z = (int)position.Z / 2;
            // This is a simplification and may need adjustment based on your map layout
            if (collision[z][x - 1] && collision[z][x - 1])
            {
                orientation = DoorOrientation.EastWest;
            }
            else
            {
                orientation = DoorOrientation.NorthSouth;
            }
        }

        // Calculate where the door should slide to when opened
        private void CalculateOpenPosition()
        {
            float slideDistance = 2.0f; // How far the door slides

            // Based on orientation, determine slide direction
            if (orientation == DoorOrientation.EastWest)
            {
                // For east-west doors, slide along the X axis
                openPosition = new Vector3(closedPosition.X + slideDistance, closedPosition.Y, closedPosition.Z);
            }
            else
            {
                // For north-south doors, slide along the Z axis
                openPosition = new Vector3(closedPosition.X, closedPosition.Y, closedPosition.Z + slideDistance);
            }
        }

        // Update method for animation - call this every frame in your game loop
        public void Update(float deltaTime)
        {
            if ((open && animationProgress < 1.0f) || (!open && animationProgress > 0.0f))
            {
                isAnimating = true;

                // Update animation progress
                if (open)
                {
                    animationProgress += slideSpeed * deltaTime;
                }
                else
                {
                    animationProgress -= slideSpeed * deltaTime;
                }

                // Clamp values
                animationProgress = Math.Clamp(animationProgress, 0.0f, 1.0f);

                // Update current position based on animation progress
                position = Vector3.Lerp(closedPosition, openPosition, animationProgress);

                // Check if animation is complete
                if (animationProgress == 0.0f || animationProgress == 1.0f)
                {
                    isAnimating = false;
                }
            }
        }

        public bool Check(Camera player, bool[][] collisions)
        {
            if ((int)player.pos.X / 2 == indexX && ((int)player.pos.Z / 2 - 1 == indexZ || (int)player.pos.Z / 2 + 1 == indexZ))
            {
                if (!open)
                {
                    collisions[indexZ][indexX] = false;
                    return true;
                }
            }
            else if ((int)player.pos.Z / 2 == indexZ && ((int)player.pos.X / 2 - 1 == indexX || (int)player.pos.X / 2 + 1 == indexX))
            {
                if (!open)
                {
                    collisions[indexZ][indexX] = false;
                    return true;
                }
            }

            // Debug info
            Console.WriteLine("Player X: " + (int)player.pos.X / 2 + " Z: " + (int)player.pos.Z / 2 + "; Doors X: " + indexX + " Z: " + indexZ);
            return false;
        }

        // Method to toggle the door state
        public void Toggle()
        {
            open = !open;

            // If we're closing the door, update the collision map
            if (!open)
            {
                // You would need to update the collisions array here if needed
                // collisions[indexZ][indexX] = true;
            }
        }
    }
}
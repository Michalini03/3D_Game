using Game3D;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
namespace Zpg
{
    public class Camera
    {
        private float zoom = 1;
        public Vector3 pos = new Vector3(0, 0, 5);
        public float rx = 0;
        public float ry = 0;
        public float speed = 1.4f;
        public float sensitivity = 1;
        public Viewport viewport;

        // Add the front vector property
        public Vector3 Front
        {
            get
            {
                // Calculate direction vector based on rotation angles
                Vector3 direction = new Vector3();
                direction.X = (float)(Math.Cos(rx) * Math.Sin(ry));
                direction.Y = -(float)(Math.Sin(rx));  // Removed the negative sign here
                direction.Z = -(float)(Math.Cos(rx) * Math.Cos(ry));
                return direction.Normalized();
            }
        }

        public Camera(Viewport viewport)
        {
            this.viewport = viewport;
        }

        public virtual Matrix4 Projection
        {
            get
            {
                float ratio = (float)((viewport.Width * viewport.window.ClientSize.X) / (viewport.Height * viewport.window.ClientSize.Y));
                return Matrix4.CreatePerspectiveFieldOfView(zoom, ratio, 0.1f, 50);
            }
        }

        public void Zoom(float coef)
        {
            //zoom *= coef;
            zoom += coef / 10.0f;
            zoom = Math.Max(0.2f, Math.Min(2, zoom));
            Console.WriteLine(zoom);
        }

        public void Move(float xdt, float ydt, bool[][] collision)
        {
            float newPosX = pos.X + speed * (float)(xdt * Math.Cos(ry) + ydt * Math.Sin(ry));
            float newPosZ = pos.Z + speed * (float)(xdt * Math.Sin(ry) - ydt * Math.Cos(ry));

            float collisionRadius = 0.20f;

            // Try to move separately on X and Z axes for wall sliding
            bool canMoveX = true;
            bool canMoveZ = true;

            // Check X movement
            float testPosX = newPosX;
            float testPosZ = pos.Z;

            // Check collision on X axis movement
            int colIndexX = (int)(testPosX / 2);
            int rowIndexX = (int)(testPosZ / 2);

            // Check surrounding cells for radius-based collision on X axis
            for (int r = -1; r <= 1; r++)
            {
                for (int c = -1; c <= 1; c++)
                {
                    int checkRow = rowIndexX + r;
                    int checkCol = colIndexX + c;

                    if (checkRow >= 0 && checkRow < collision.Length &&
                        checkCol >= 0 && checkCol < collision[checkRow].Length &&
                        collision[checkRow][checkCol])
                    {
                        // Calculate center of the cell
                        float cellCenterX = (checkCol * 2) + 1;
                        float cellCenterZ = (checkRow * 2) + 1;

                        // Calculate distance from player to cell center
                        float distX = Math.Abs(testPosX - cellCenterX);
                        float distZ = Math.Abs(testPosZ - cellCenterZ);

                        // If too close to wall center, block X movement
                        if (distX < collisionRadius + 1 && distZ < collisionRadius + 1)
                        {
                            canMoveX = false;
                            break;
                        }
                    }
                }
                if (!canMoveX) break;
            }

            // Check Z movement
            testPosX = pos.X;
            testPosZ = newPosZ;

            // Check collision on Z axis movement
            int colIndexZ = (int)(testPosX / 2);
            int rowIndexZ = (int)(testPosZ / 2);

            // Check surrounding cells for radius-based collision on Z axis
            for (int r = -1; r <= 1; r++)
            {
                for (int c = -1; c <= 1; c++)
                {
                    int checkRow = rowIndexZ + r;
                    int checkCol = colIndexZ + c;

                    if (checkRow >= 0 && checkRow < collision.Length &&
                        checkCol >= 0 && checkCol < collision[checkRow].Length &&
                        collision[checkRow][checkCol])
                    {
                        // Calculate center of the cell
                        float cellCenterX = (checkCol * 2) + 1;
                        float cellCenterZ = (checkRow * 2) + 1;

                        // Calculate distance from player to cell center
                        float distX = Math.Abs(testPosX - cellCenterX);
                        float distZ = Math.Abs(testPosZ - cellCenterZ);

                        // If too close to wall center, block Z movement
                        if (distX < collisionRadius + 1 && distZ < collisionRadius + 1)
                        {
                            canMoveZ = false;
                            break;
                        }
                    }
                }
                if (!canMoveZ) break;
            }

            // Apply movement based on what's allowed
            if (canMoveX)
            {
                pos.X = pos.X + speed * (float)(xdt * Math.Cos(ry) + ydt * Math.Sin(ry));
            }

            if (canMoveZ)
            {
                pos.Z = pos.Z + speed * (float)(xdt * Math.Sin(ry) - ydt * Math.Cos(ry));
            }
        }

        public void RotateX(float a)
        {
            rx += a * sensitivity;
            rx = (float)Math.Max(-Math.PI / 2, Math.Min(Math.PI / 2, rx));
        }

        public void RotateY(float a)
        {
            ry += a * sensitivity;
        }

        public virtual Matrix4 View
        {
            get
            {
                Matrix4 view;
                view = Matrix4.Identity;
                view *= Matrix4.CreateTranslation(-pos);
                view *= Matrix4.CreateRotationY(ry);
                view *= Matrix4.CreateRotationX(rx);
                return view;
            }
        }
    }
}
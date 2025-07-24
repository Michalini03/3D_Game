using Game3D;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

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
        public int k = 0;

        public float whiteFade = 0;
        public Vector3 Front
        {
            get
            {
                Vector3 direction = new Vector3();
                direction.X = (float)(Math.Cos(rx) * Math.Sin(ry));
                direction.Y = -(float)(Math.Sin(rx));
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

        public void Move(float xdt, float ydt, bool[][][] collision)
        {
            float newPosX = pos.X + speed * (float)(xdt * Math.Cos(ry) + ydt * Math.Sin(ry));
            float newPosZ = pos.Z + speed * (float)(xdt * Math.Sin(ry) - ydt * Math.Cos(ry));
            float collisionRadius = 0.20f;

            int mapWidth = collision[0].Length * 2;
            int mapHeight = collision.Length * 2;

            if (newPosX < collisionRadius || newPosX > mapWidth - collisionRadius)
            {
                newPosX = Math.Clamp(newPosX, collisionRadius, mapWidth - collisionRadius);
            }

            if (newPosZ < collisionRadius || newPosZ > mapHeight - collisionRadius)
            {
                newPosZ = Math.Clamp(newPosZ, collisionRadius, mapHeight - collisionRadius);
            }

            bool canMoveX = true;
            bool canMoveZ = true;

            float testPosX = newPosX;
            float testPosZ = pos.Z;

            int colIndexX = (int)(testPosX / 2);
            int rowIndexX = (int)(testPosZ / 2);

            if (rowIndexX >= 0 && rowIndexX < collision.Length &&
                colIndexX >= 0 && colIndexX < collision[0].Length)
            {
                for (int r = -1; r <= 1; r++)
                {
                    for (int c = -1; c <= 1; c++)
                    {
                        int checkRow = rowIndexX + r;
                        int checkCol = colIndexX + c;

                        if (checkRow >= 0 && checkRow < collision.Length &&
                            checkCol >= 0 && checkCol < collision[checkRow].Length &&
                            collision[checkRow][checkCol][this.k])
                        {
                            float cellCenterX = (checkCol * 2) + 1;
                            float cellCenterZ = (checkRow * 2) + 1;

                            float distX = Math.Abs(testPosX - cellCenterX);
                            float distZ = Math.Abs(testPosZ - cellCenterZ);

                            if (distX < collisionRadius + 1 && distZ < collisionRadius + 1)
                            {
                                canMoveX = false;
                                break;
                            }
                        }
                    }
                    if (!canMoveX) break;
                }
            }
            else
            {
                canMoveX = false;
            }

            testPosX = pos.X;
            testPosZ = newPosZ;

            int colIndexZ = (int)(testPosX / 2);
            int rowIndexZ = (int)(testPosZ / 2);

            if (rowIndexZ >= 0 && rowIndexZ < collision.Length &&
                colIndexZ >= 0 && colIndexZ < collision[0].Length)
            {
                for (int r = -1; r <= 1; r++)
                {
                    for (int c = -1; c <= 1; c++)
                    {
                        int checkRow = rowIndexZ + r;
                        int checkCol = colIndexZ + c;

                        if (checkRow >= 0 && checkRow < collision.Length &&
                            checkCol >= 0 && checkCol < collision[checkRow].Length &&
                            collision[checkRow][checkCol][this.k])
                        {
                            float cellCenterX = (checkCol * 2) + 1;
                            float cellCenterZ = (checkRow * 2) + 1;

                            float distX = Math.Abs(testPosX - cellCenterX);
                            float distZ = Math.Abs(testPosZ - cellCenterZ);

                            if (distX < collisionRadius + 1 && distZ < collisionRadius + 1)
                            {
                                canMoveZ = false;
                                break;
                            }
                        }
                    }
                    if (!canMoveZ) break;
                }
            }
            else
            {
                canMoveZ = false;
            }

            if (canMoveX)
            {
                pos.X = newPosX;
            }

            if (canMoveZ)
            {
                pos.Z = newPosZ;
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
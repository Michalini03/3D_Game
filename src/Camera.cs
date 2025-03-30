using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Game3D
{
    public class Camera
    {
        private float zoom = 1;
        public Vector3 pos;
        public Vector3 front;
        public float rx;
        public float ry;
        public float speed = 1.4f;
        public float sensitivity = 1;
        
        public bool[][] collisions { get; set;}

        public Viewport viewport;
        public Camera(Viewport viewport, Vector3 pos)
        {
            this.viewport = viewport;
            this.pos = pos;
            this.rx = 0;
            this.ry = 0;
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

        public void Move(float xdt, float ydt)
        {
            float newPosX = pos.X + speed * (float)(xdt * Math.Cos(ry) + ydt * Math.Sin(ry));
            float newPosZ = pos.Z + speed * (float)(xdt * Math.Sin(ry) - ydt * Math.Cos(ry));

            float collisionRadius = 0.15f;

            int newIndexXLeft = (int)((newPosX - collisionRadius) / 2);
            int newIndexXRight = (int)((newPosX + collisionRadius) / 2);
            int newIndexZUp = (int)((newPosZ - collisionRadius) / 2);
            int newIndexZDown = (int)((newPosZ + collisionRadius) / 2);

            if (newIndexXLeft < 0) newIndexXLeft = 0;
            if (newIndexXRight >= collisions[0].Length) newIndexXRight = collisions[0].Length - 1;
            if (newIndexZUp < 0) newIndexZUp = 0;
            if (newIndexZDown >= collisions.Length) newIndexZDown = collisions.Length - 1;

            bool collisionDetected = collisions[newIndexZUp][newIndexXLeft] ||
                                     collisions[newIndexZUp][newIndexXRight] ||
                                     collisions[newIndexZDown][newIndexXLeft] ||
                                     collisions[newIndexZDown][newIndexXRight];

            if (!collisionDetected)
            {
                pos.X += speed * (float)(xdt * Math.Cos(ry) + ydt * Math.Sin(ry));
                pos.Z += speed * (float)(xdt * Math.Sin(ry) - ydt * Math.Cos(ry));
            }
            else
            {
                if ((int)pos.Z / 2 != newIndexZUp || (int)pos.Z / 2 != newIndexZDown) { 
                    pos.X += speed * (float)(xdt * Math.Cos(ry) + ydt * Math.Sin(ry));
                }
                else if ((int)pos.X / 2 != newIndexXLeft || (int)pos.X / 2 != newIndexXRight)
                {
                    pos.Z += speed * (float)(xdt * Math.Sin(ry) - ydt * Math.Cos(ry));
                }
            }
        }

        public void RotateX(float a)
        {
            rx += a * sensitivity;
            rx = (float)Math.Max(-Math.PI / 2, Math.Min(Math.PI / 2, rx));
            updateFront();
        }

        public void RotateY(float a)
        {
            ry += a * sensitivity;
            updateFront();
        }

        public void updateFront()
        {
            front.X = (float)Math.Cos(MathHelper.DegreesToRadians(ry)) * (float)Math.Cos(MathHelper.DegreesToRadians(rx));
            front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(ry));
            front.Z = (float)Math.Cos(MathHelper.DegreesToRadians(ry)) * (float)Math.Sin(MathHelper.DegreesToRadians(rx));
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

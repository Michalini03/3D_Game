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
        const float depressionAngle = 2.0f;

        private Vector3 _front = -Vector3.UnitZ;

        private Vector3 _up = Vector3.UnitY;

        private Vector3 _right = Vector3.UnitX;
        
        private float _pitch;
        private float _yaw = -MathHelper.PiOver2;

        private float _fov = MathHelper.PiOver2;

        public float speed = 1.4f;
        public float sensitivity = 1;
        
        public bool[][] collisions { get; set;}

        public Viewport viewport;
        public Camera(Viewport viewport, Vector3 pos)
        {
            this.viewport = viewport;
            this.position = pos;
        }

        public Vector3 position { get; set; }

        public virtual Matrix4 Projection
        {
            get
            {
                float ratio = (float)((viewport.Width * viewport.window.ClientSize.X) / (viewport.Height * viewport.window.ClientSize.Y));
                return Matrix4.CreatePerspectiveFieldOfView(_fov, ratio, 0.1f, 50);
            }
        }

        public Vector3 Front => _front;

        public Vector3 Up => _up;

        public Vector3 Right => _right;

        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                // We clamp the pitch value between -89 and 89 to prevent the camera from going upside down, and a bunch
                // of weird "bugs" when you are using euler angles for rotation.
                // If you want to read more about this you can try researching a topic called gimbal lock
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.DegreesToRadians(angle);
                MathHelper.DegreesToRadians(depressionAngle);
                UpdateVectors();
            }
        }

        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }

        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 90f);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }

        public virtual Matrix4 View
        {
            get
            {
                Matrix4 view;
                view = Matrix4.LookAt(position, position + _front, _up);
                return view;
            }

        }

        private void UpdateVectors()
        {
            _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            _front.Y = MathF.Sin(_pitch);
            _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

            _front = Vector3.Normalize(_front);

            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Game3D
{
    public class Light
    {
        public Vector4 position;
        public Vector3 color;
        public float intensity;

        public Light(Vector3 posOrDir, bool isDirectional, float intensity)
        {
            position = new Vector4(posOrDir, isDirectional ? 0f : 1f);
            color = Vector3.One;
            this.intensity = intensity;
        }
    }
}

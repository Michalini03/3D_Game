using Game3D;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zpg
{
    public class Material
    {
        public Vector3 diffuse = new Vector3(0.5f, 0.5f, 0.5f);
        public Vector3 specular = new Vector3(0.8f, 0.8f, 0.8f);
        public float shininess = 32.0f;

        public void SetUniforms(Shader shader)
        {
            shader.SetUniform("material.diffuse", diffuse);
            shader.SetUniform("material.specular", specular);
            shader.SetUniform("material.shininess", shininess);
        }
    }
}

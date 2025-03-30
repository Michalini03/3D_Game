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
        public Vector3 ambient;
        public Vector3 diffuse;
        public Vector3 specular;
        public float shininess;

        public Material(Vector3 ambient, Vector3 diffuse, Vector3 specular, float shininess) {
            this.ambient = ambient;
            this.diffuse = diffuse;
            this.specular = specular;
            this.shininess = shininess;
        }

        public void SetUniforms(Shader shader)
        {
            shader.SetUniform("material.ambient", ambient);
            shader.SetUniform("material.diffuse", diffuse);
            shader.SetUniform("material.specular", specular);
            shader.SetUniform("material.shininess", shininess);
        }
    }

}

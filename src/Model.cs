using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Zpg;

namespace Game3D
{
    /// <summary>
    /// Jednoduchý model obsahující seznam vrcholů
    /// </summary>
    public class Model : IDisposable
    {
        public Shader Shader { get; set; }

        public Material Material { get; set; }

        public Vector3 position { get; set; }

        public int vbo;       
        public int ibo;
        public int vao;

        public int triangles;

        protected void Create(Vertex[] vertices, int[] indices)
        {
            triangles = indices.Length / 3;

            // vytvoření a připojení VAO
            GL.GenVertexArrays(1, out vao);
            GL.BindVertexArray(vao);

            // Vytvoření a připojení VBO
            vbo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Vertex.SizeOf(), vertices, BufferUsageHint.StaticDraw);

            // vytvoření a připojení IBO
            ibo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);

            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

            // namapování pointerů na lokace v shaderu
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.SizeOf(), IntPtr.Zero);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex.SizeOf(), (IntPtr)3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
        }


        /// <summary>
        /// Vykreslení modelu
        /// </summary>
        public void Draw(Camera camera, bool toogle)
        {
            Matrix4 translate = Matrix4.CreateTranslation((float)position.X, (float)position.Y, (float)position.Z);

            Shader.Use();
            Shader.SetUniform("projection", camera.Projection);
            Shader.SetUniform("view", camera.View);
            Shader.SetUniform("model", translate);

            Shader.SetUniform("cameraPosWorld", camera.pos);
            Material.SetUniforms(Shader);

            const float flashlightHeight = 2.05f;
            float depressionAngle = 2.0f;

            float depressionRad = depressionAngle * (float)Math.PI / 180.0f;

            Vector3 rotatedDirection = new Vector3(
                camera.Front.X,
                camera.Front.Y * (float)Math.Cos(depressionRad) - camera.Front.Z * (float)Math.Sin(depressionRad),
                camera.Front.Y * (float)Math.Sin(depressionRad) + camera.Front.Z * (float)Math.Cos(depressionRad)
            );
            rotatedDirection = Vector3.Normalize(rotatedDirection);

            Shader.SetUniform("light.position", new Vector4(camera.pos.X, flashlightHeight, camera.pos.Z, 1));
            Shader.SetUniform("light.direction", rotatedDirection);
            Shader.SetUniform("light.cutOff", (float)Math.Cos(MathHelper.DegreesToRadians(15)));
            Shader.SetUniform("light.outerCutOff", (float)Math.Cos(MathHelper.DegreesToRadians(20)));
            if (toogle)
                Shader.SetUniform("light.color", Vector3.One);
            else
                Shader.SetUniform("light.color", Vector3.Zero);
            /**
            Shader.SetUniform("light.ambient", new Vector3(0.2f, 0.2f, 0.2f));
            Shader.SetUniform("light.diffuse", new Vector3(0.5f, 0.5f, 0.5f));
            Shader.SetUniform("light.specular", new Vector3(1.0f, 1.0f, 1.0f));*/

            // Připojení bufferu
            GL.BindVertexArray(vao);

            //GL.Enable(EnableCap.CullFace);
            //GL.LineWidth(5);
            GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);

            // Vykreslení pole vrcholů
            GL.DrawElements(PrimitiveType.Triangles, 3*triangles, DrawElementsType.UnsignedInt, IntPtr.Zero);
            GL.BindVertexArray(0);
        }

        public static void SimpleNormals(Vertex[] vertices, int[] indices)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].normal = Vector3.Zero;
            }

            for (int i = 0; i < indices.Length; i += 3)
            {
                int i0 = indices[i];
                int i1 = indices[i + 1];
                int i2 = indices[i + 2];

                Vector3 v1 = vertices[i1].position - vertices[i0].position;
                Vector3 v2 = vertices[i2].position - vertices[i0].position;

                Vector3 norm = Vector3.Cross(v1, v2).Normalized();

                vertices[i0].normal += norm;
                vertices[i1].normal += norm;
                vertices[i2].normal += norm;
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                if (vertices[i].normal.LengthSquared > 0.0001f)
                {
                    vertices[i].normal.Normalize();
                }
                else
                {
                    vertices[i].normal = new Vector3(0, 1, 0);
                }
            }
        }

        #region Dispose - uvolnění paměti
        bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {

                }
                GL.DeleteBuffer(vbo);
                GL.DeleteBuffer(ibo);
                GL.DeleteVertexArray(vao);
                disposed = true;
            }
        }

        ~Model() => Dispose(false);
        #endregion
    }
}

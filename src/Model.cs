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
        public Vertex[] vertices { get; set; }
        public int[] indices   { get; set; }

        public Shader Shader { get; set; }

        public Material Material { get; set; }

        public Vector3 position { get; set; }

        public int vbo;       
        public int ibo;
        public int vao;

        public int triangles;

        public void Bind()
        {
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

                // namapování pointerů na lokace v shaderu + normaly + zapnuti odpovidajiciho atributu
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.SizeOf(), IntPtr.Zero);
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex.SizeOf(), (IntPtr)3 * sizeof(float));
                GL.EnableVertexAttribArray(1);

                GL.BindVertexArray(0);
            }
        }


        /// <summary>
        /// Vykreslení modelu
        /// </summary>
        public void Draw(Camera camera)
        {
            Matrix4 translate = Matrix4.CreateTranslation((float)position.X, (float)position.Y, (float)position.Z);

            Shader.Use();
            Shader.SetUniform("projection", camera.Projection);
            Shader.SetUniform("view", camera.View);
            Shader.SetUniform("model", translate);

            Shader.SetUniform("cameraPosWorld", camera.position);

            Material.SetUniforms(Shader);

            const float flashlightHeight = 2.05f;
            float depressionAngle = 2.0f;


            Shader.SetUniform("light.position", new Vector3(camera.position.X, flashlightHeight, camera.position.Z));
            Shader.SetUniform("light.direction", camera.Front);
            Shader.SetUniform("light.cutOff", (float)Math.Cos(MathHelper.DegreesToRadians(20.5f)));
            Shader.SetUniform("light.ambient", new Vector3(0.2f, 0.2f, 0.2f));
            Shader.SetUniform("light.diffuse", new Vector3(0.7f, 0.7f, 0.7f));
            Shader.SetUniform("light.specular", new Vector3(1.0f, 1.0f, 1.0f));

            


            // Připojení bufferu
            GL.BindVertexArray(vao);

            //GL.Enable(EnableCap.CullFace);
            //GL.LineWidth(5);
            GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);

            // Vykreslení pole vrcholů
            GL.DrawElements(PrimitiveType.Triangles, 3*triangles, DrawElementsType.UnsignedInt, IntPtr.Zero);
            GL.BindVertexArray(0);
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
            }
        }

        ~Model() => Dispose(false);
        #endregion
    }
}

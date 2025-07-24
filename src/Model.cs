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

        public Dictionary<string, Texture> Textures { get; set; } = new();

        public Vector3 position { get; set; }

        public int vbo;       
        public int ibo;
        public int vao;

        public int triangles;

        public float rotationAngle = 0.0f;

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
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vertex.SizeOf(), (IntPtr)6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            GL.BindVertexArray(0);
        }


        /// <summary>
        /// Vykreslení modelu
        /// </summary>
        public void Draw(Camera camera, Light light, bool toogle, bool lightMode, bool isMinimap = false)
        {
            Matrix4 rotation = Matrix4.CreateRotationY(rotationAngle);
            Matrix4 translate = Matrix4.CreateTranslation((float)position.X, (float)position.Y, (float)position.Z);
            Matrix4 world = rotation * translate;

            Shader.Use();
            Shader.SetUniform("projection", camera.Projection);
            Shader.SetUniform("view", camera.View);
            Shader.SetUniform("model", world);

            Shader.SetUniform("cameraPosWorld", camera.pos);
            Material.SetUniforms(Shader);

            if (isMinimap)
            {
                // Simple directional light from above
                Shader.SetUniform("light.position", new Vector4(0, 70, 0, 0)); // Directional light (w=0)
                Shader.SetUniform("light.direction", new Vector3(0, -1, 0));   // Shine downward
                Shader.SetUniform("light.color", new Vector3(1.0f, 1.0f, 1.0f)); // White light
                Shader.SetUniform("light.cutOff", (float)Math.Cos(MathHelper.DegreesToRadians(20))); // Disable spotlight cone
                Shader.SetUniform("light.outerCutOff", (float)Math.Cos(MathHelper.DegreesToRadians(25)));
                Shader.SetUniform("ambientStrength", 1f);
                Shader.SetUniform("whiteFade", 0f);
            }
            else
            {
                // Flashlight logic
                float flashlightHeight = 2.05f + (camera.pos.Y - 1.7f);
                float depressionAngle = 2.0f;
                float depressionRad = MathHelper.DegreesToRadians(depressionAngle);

                Vector3 rotatedDirection = new Vector3(
                    camera.Front.X,
                    camera.Front.Y * (float)Math.Cos(depressionRad) - camera.Front.Z * (float)Math.Sin(depressionRad),
                    camera.Front.Y * (float)Math.Sin(depressionRad) + camera.Front.Z * (float)Math.Cos(depressionRad)
                );
                rotatedDirection = Vector3.Normalize(rotatedDirection);

                Shader.SetUniform("light.position", new Vector4(camera.pos.X, flashlightHeight, camera.pos.Z, 1));
                Shader.SetUniform("light.direction", rotatedDirection);
                Shader.SetUniform("light.cutOff", (float)Math.Cos(MathHelper.DegreesToRadians(20)));
                Shader.SetUniform("light.outerCutOff", (float)Math.Cos(MathHelper.DegreesToRadians(25)));
                Shader.SetUniform("light.color", toogle ? Vector3.One : Vector3.Zero);
                Shader.SetUniform("ambientStrength", 0.01f);
                Shader.SetUniform("whiteFade", camera.whiteFade);
            }

            int unit = 0;
            foreach (var kw in Textures)
            {
                kw.Value.Bind(Shader.uniforms[kw.Key], unit++);
            }

            // Připojení bufferu
            GL.BindVertexArray(vao);

            //GL.Enable(EnableCap.CullFace);
            //GL.LineWidth(5);

            if (!lightMode) GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
            else GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);

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
            }
        }

        ~Model() => Dispose(false);
        #endregion
    }
}

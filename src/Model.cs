﻿using OpenTK.Graphics.OpenGL;
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
        public void Draw(Camera camera)
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


            Shader.SetUniform("light.position", new Vector4(camera.pos.X, flashlightHeight, camera.pos.Z, 1));
            Shader.SetUniform("light.direction", camera.Front);
            Shader.SetUniform("light.cutOff", (float)Math.Cos(MathHelper.DegreesToRadians(15)));
            Shader.SetUniform("light.outerCutOff", (float)Math.Cos(MathHelper.DegreesToRadians(20)));
            Shader.SetUniform("light.color", Vector3.One);
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
            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector3 v1 = vertices[indices[i + 1]].position - vertices[indices[i]].position;
                Vector3 v2 = vertices[indices[i + 2]].position - vertices[indices[i]].position;

                Vector3 norm = Vector3.Cross(v1, v2).Normalized();

                vertices[indices[i]].normal += norm;
                vertices[indices[i + 1]].normal += norm;
                vertices[indices[i + 2]].normal += norm;
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].normal.Normalize();
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

using Game3D;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using Zpg;

public class MinimapCamera : Camera
{

    public Vector3 up = new Vector3(0, 0, -1);
    public MinimapCamera(Viewport viewport) : base(viewport)
    {
    }

    public override Matrix4 Projection
    {
        get
        {
            // Orthographic projection (top-down view)
            float size = 10f;
            float aspectRatio = (float)((viewport.Width * viewport.window.ClientSize.X) / (viewport.Height * viewport.window.ClientSize.Y));
            return Matrix4.CreateOrthographic(size * aspectRatio, size, 0.1f, 100f);
        }
    }

    public override Matrix4 View
    {
        get
        {
            // Look straight down from above
            Vector3 topDownPos = new Vector3(pos.X, 20.0f, pos.Z); // move up high
            Vector3 lookAt = new Vector3(pos.X, 0, pos.Z);         // look directly down

            return Matrix4.LookAt(topDownPos, lookAt, this.up);
        }
    }

    private int playerMarkerVAO, playerMarkerVBO, playerMarkerEBO;
    private Shader markerShader;

    public void InitializePlayerMarker()
    {
        float[] playerMarkerVertices = new float[]
        {
         0.0f,  0.08f,   // Vertex 0: Top point
        -0.05f, -0.03f,  // Vertex 1: Bottom left
         0.05f, -0.03f   // Vertex 2: Bottom right
        };

        uint[] playerMarkerIndices = new uint[]
        {
        0, 1, 2
        };

        playerMarkerVAO = GL.GenVertexArray();
        playerMarkerVBO = GL.GenBuffer();
        playerMarkerEBO = GL.GenBuffer();
        GL.BindVertexArray(playerMarkerVAO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, playerMarkerVBO);
        GL.BufferData(BufferTarget.ArrayBuffer, playerMarkerVertices.Length * sizeof(float), playerMarkerVertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, playerMarkerEBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, playerMarkerIndices.Length * sizeof(uint), playerMarkerIndices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        markerShader = new Shader("shaders/marker.vert", "shaders/marker.frag");
    }

    public void DrawPlayerMarker(Vector3 playerWorldPosition)
    {
        // First, get the player's position in normalized device coordinates (NDC).
        Matrix4 view = this.View;
        Matrix4 projection = this.Projection;

        Vector4 clipSpacePosition = projection * view * new Vector4(playerWorldPosition, 1.0f);

        Vector3 ndcPosition = new Vector3(
            clipSpacePosition.X / clipSpacePosition.W,
            clipSpacePosition.Y / clipSpacePosition.W,
            clipSpacePosition.Z / clipSpacePosition.W
        );

        float normalizedX = (ndcPosition.X + 1.0f) / 2.0f;
        float normalizedY = (ndcPosition.Y + 1.0f) / 2.0f;

        float minimapX = normalizedX * (float)viewport.Width + (float)viewport.Left;
        float minimapY = normalizedY * (float)viewport.Height + (float)viewport.Top;

        markerShader.Use();


        // Disable depth test so the marker draws over everything
        GL.Disable(EnableCap.DepthTest);

        GL.BindVertexArray(playerMarkerVAO);
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);

        GL.Enable(EnableCap.DepthTest);
    }
}

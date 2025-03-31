using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Zpg;
using Zpg.models;

namespace Game3D;



public class Program : GameWindow
{
    public static void Main()
    {
        using (var program = new Program())
        {
            program.VSync = VSyncMode.On;
            program.Run();
        }
    }

    const float PLAYER_VIEW = 1.7f;
    const float FLASHLIGHT_HEIGHT = 2.07f;

    Viewport mainViewport;
    Camera mainCamera;

    List<Model> walls = new List<Model>();
    List<Door> doors = new List<Door>();
    List<Model> collectibles = new List<Model>();
    Model plane;

    Light light;
    bool grabbed;

    int mapSizeX;
    int mapSizeZ;

    private double elapsedTime = 0.0;
    private int frameCount = 0;
    private double currentFps = 0.0;

    public Program() : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize=new OpenTK.Mathematics.Vector2i(1366,768) }) { }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.5f, 0.7f, 1.0f, 1.0f);

        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);
        GL.DebugMessageCallback(DebugCallback, IntPtr.Zero);
        GL.Enable(EnableCap.DepthTest);

        CursorState = CursorState.Grabbed;
        grabbed = true;

        mainViewport = new Viewport()
        {
            Left = 0,
            Top = 0,
            Width = 1,
            Height = 1,
            window = this
        };

        this.LoadMap("maps/test.md");
        light = new Light(new Vector3(-mapSizeX / 2, -1, -mapSizeZ / 2), false, 0.3f);        
    }

    private static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
    {
        string msg = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(message, length);
        Console.WriteLine($"OpenGL Debug Message: {msg}\nSource: {source}, Type: {type}, Severity: {severity}, ID: {id}");
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        mainViewport.Set();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        elapsedTime += args.Time;
        frameCount++;

        if (elapsedTime >= 1.0)
        {
            currentFps = frameCount / elapsedTime;
            Title = $"FPS: {currentFps:F2}";

            frameCount = 0;
            elapsedTime = 0.0;
        }

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        plane.Draw(mainCamera);
        for (int i = 0; i < walls.Count; i++)
        {
            walls[i].Draw(mainCamera);
        }
        for (int i = 0; i < doors.Count; i++)
        {
            if(!doors[i].open) doors[i].Draw(mainCamera);
        }
        for (int i = 0; i < collectibles.Count; i++)
        {
            collectibles[i].Draw(mainCamera);
        }
        SwapBuffers();
    }
   
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        Vector2 direction = new Vector2(0, 0);

        if (KeyboardState.IsKeyDown(Keys.Escape)) Close();
        if (KeyboardState.IsKeyDown(Keys.Tab))
        {
            if (grabbed)
            {
                CursorState = CursorState.Normal;
                grabbed = false;
            }
            else
            {
                CursorState = CursorState.Grabbed;
                grabbed = true;
            }
        }

        if (KeyboardState.IsKeyDown(Keys.E))
        {
            for (int i = 0; i < doors.Count; i++)
            {
                if (doors[i].open) continue;
                if (doors[i].Check(mainCamera))
                {
                    doors[i].open = true;
                    break;
                }
            }
        }

        if (KeyboardState.IsKeyDown(Keys.W)) direction += Vector2.UnitY;
        if (KeyboardState.IsKeyDown(Keys.S)) direction -= Vector2.UnitY;
        if (KeyboardState.IsKeyDown(Keys.A)) direction -= Vector2.UnitX;
        if (KeyboardState.IsKeyDown(Keys.D)) direction += Vector2.UnitX;

        var len = direction.Length;
        if (len > 0)
        {
            direction *= (float)args.Time / len;
            mainCamera.Move(direction.X, direction.Y);
        }

        if(mouseDelta.LengthSquared > 0.00001)
        {
            mainCamera.RotateX(mouseDelta.Y*(float)args.Time);
            mainCamera.RotateY(mouseDelta.X*(float)args.Time);
            mouseDelta = Vector2.Zero;
        }

        mainCamera.setDirection();

    }

    Vector2 mouseDelta;
    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        mouseDelta += new Vector2(e.DeltaX, e.DeltaY);
    }

    // Loads the map from .md file
    protected void LoadMap(string filename)
    {
        using (StreamReader reader = new StreamReader(filename))
        {
            string line = reader.ReadLine();
            string[] mapsSize = line.Split('x');

            mapSizeX = int.Parse(mapsSize[0]) * 2;
            mapSizeZ = int.Parse(mapsSize[1]) * 2;

            plane = new Plane(new Vector3(mapSizeX / 2f, 0f, mapSizeZ / 2f), mapSizeX, mapSizeZ);
            plane.Shader = new Shader("basic.vert", "basic.frag");

            bool[][] collisions = new bool[mapSizeX / 2][];
            for (int i = 0; i < mapSizeX / 2; i++)
            {
                collisions[i] = new bool[mapSizeZ / 2];
            }

            int z = 0;
            while ((line = reader.ReadLine()) != null)
            {
                for(int x = 0; x < line.Length; x++)
                {
                    char cell = line[x];
                    Vector3 position = new Vector3((x * 2) + 1, 0, (z * 2) + 1);

                    if (cell == '@')
                    {
                        Vector3 spawnPosition = new Vector3(position.X, PLAYER_VIEW, position.Z);
                        mainCamera = new Camera(mainViewport, spawnPosition);
                        collisions[z][x] = false;
                    }
                    else if (cell >= 'o' && cell <= 'z')
                    {
                        walls.Add(new Wall(position));
                        walls[^1].Shader = new Shader("basic.vert", "basic.frag");
                        collisions[z][x] = true;
                    }
                    else if (cell >= 'A' && cell <= 'G')
                    {
                        doors.Add(new Door(position, x, z));
                        doors[^1].Shader = new Shader("basic.vert", "basic.frag");
                        collisions[z][x] = true;
                    }
                    else if (cell >= 'T' && cell <= 'Z')
                    {
                        Vector3 specialPosition = position;
                        specialPosition.Y = 0;
                        collectibles.Add(new SimpleObj("coinBagTest.obj", specialPosition));
                        collectibles[^1].Shader = new Shader("basic.vert", "basic.frag");
                        collisions[z][x] = false;
                    }
                    else
                    {
                        collisions[z][x] = false;
                    }
                }
                z++;
            }
            mainCamera.collisions = collisions;
        }
        return;
    }
}
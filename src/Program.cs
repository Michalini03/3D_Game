using System;
using System.Reflection;
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

    bool[][] collisions;

    Viewport mainViewport;
    Camera mainCamera;

    List<Model> walls = new List<Model>();
    List<Door> doors = new List<Door>();
    List<Model> collectibles = new List<Model>();
    Model plane;

    Light light;
    bool grabbed;
    bool toogle;

    int mapSizeX;
    int mapSizeZ;

    private bool _firstMove = true;
    private Vector2 _lastPos;

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

        toogle = true;

        mainViewport = new Viewport()
        {
            Left = 0,
            Top = 0,
            Width = 1,
            Height = 1,
            window = this
        };

        this.LoadMap("maps/basic.md");
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
        plane.Draw(mainCamera, toogle);
        for (int i = 0; i < walls.Count; i++)
        {
            walls[i].Draw(mainCamera, toogle);
        }
        for (int i = 0; i < doors.Count; i++)
        {
            doors[i].Draw(mainCamera, toogle);
        }
        for (int i = 0; i < collectibles.Count; i++)
        {
            collectibles[i].Draw(mainCamera, toogle);
        }
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        // Get deltaTime directly from FrameEventArgs
        float deltaTime = (float)e.Time; // This is in seconds

        // Update all doors
        for (int i = 0; i < doors.Count; i++)
        {
            doors[i].Update(deltaTime);
        }

        Vector2 direction = new Vector2(0, 0);
        if (KeyboardState.IsKeyDown(Keys.Escape)) Close();
        if (KeyboardState.IsKeyDown(Keys.W)) direction += Vector2.UnitY;
        if (KeyboardState.IsKeyDown(Keys.S)) direction -= Vector2.UnitY;
        if (KeyboardState.IsKeyDown(Keys.A)) direction -= Vector2.UnitX;
        if (KeyboardState.IsKeyDown(Keys.D)) direction += Vector2.UnitX;
        if (KeyboardState.IsKeyPressed(Keys.F)) toogle = !toogle;
        if (KeyboardState.IsKeyPressed(Keys.E))
        {
            for (int i = 0; i < doors.Count; i++)
            {
                // Check if player is near the door and can interact with it
                if (doors[i].Check(mainCamera, collisions))
                {
                    // Toggle the door state
                    doors[i].Toggle();
                    doors[i].open = true;
                    break;
                }
            }
        }

        var len = direction.Length;
        if (len > 0)
        {
            direction *= deltaTime / len; // Using the deltaTime we got from e.Time
            mainCamera.Move(direction.X, direction.Y, collisions);
        }

        if (mouseDelta.LengthSquared > 0.00001)
        {
            mainCamera.RotateX(mouseDelta.Y * deltaTime); // Using the deltaTime here too
            mainCamera.RotateY(mouseDelta.X * deltaTime);
            mouseDelta = Vector2.Zero;
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        mainCamera.Zoom(-e.OffsetY);
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
            plane.Material = new Material() { diffuse = new Vector3(0.5f, 0.5f, 0.5f), specular = new Vector3(0.8f, 0.8f, 0.8f), shininess = 50 };

            collisions = new bool[mapSizeZ / 2][];
            for (int i = 0; i < mapSizeZ / 2; i++)
            {
                collisions[i] = new bool[mapSizeX / 2];
            }

            int z = 0;
            while ((line = reader.ReadLine()) != null)
            {
                for(int x = 0; x < mapSizeX/2; x++)
                {
                    char cell = line[x];
                    Vector3 position = new Vector3((x * 2) + 1, 0, (z * 2) + 1);

                    if (cell == '@')
                    {
                        Vector3 spawnPosition = new Vector3(position.X, PLAYER_VIEW, position.Z);
                        mainCamera = new Camera(mainViewport);
                        mainCamera.pos = spawnPosition;
                        collisions[z][x] = false;
                    }
                    else if (cell >= 'o' && cell <= 'z')
                    {
                        walls.Add(new Wall(position));
                        walls[^1].Shader = new Shader("basic.vert", "basic.frag");
                        walls[^1].Material = new Material() { diffuse = new Vector3(0.8f, 0.2f, 0f), specular = new Vector3(0.8f, 0.8f, 0.1f), shininess = 50 };

                        collisions[z][x] = true;
                    }
                    else if (cell >= 'A' && cell <= 'G')
                    {
                        doors.Add(new Door(position, x, z, collisions));
                        doors[^1].Shader = new Shader("basic.vert", "basic.frag");
                        doors[^1].Material = new Material() { diffuse = new Vector3(0.5f, 0.3f, 0.0f), specular = new Vector3(0.8f, 0.8f, 0.1f), shininess = 50 };

                        collisions[z][x] = true;
                    }
                    else if (cell >= 'T' && cell <= 'Z')
                    {
                        Vector3 specialPosition = position;
                        specialPosition.Y = 0;
                        collectibles.Add(new SimpleObj("sphere.obj", specialPosition));
                        collectibles[^1].Shader = new Shader("basic.vert", "basic.frag");
                        collectibles[^1].Material = new Material() { diffuse = new Vector3(0.7f, 0.5f, 0.0f), specular = new Vector3(0.8f, 0.8f, 0.1f), shininess = 50 };

                        collisions[z][x] = false;
                    }
                    else
                    {
                        collisions[z][x] = false;
                    }
                }
                z++;
            }
        }
        return;
    }
}
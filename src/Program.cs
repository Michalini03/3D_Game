using System;
using System.Buffers.Text;
using System.ComponentModel.Design;
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
            program.VSync = VSyncMode.Off;
            program.ClientSize = new Vector2i(1366, 768);
            program.Run();
        }
    }

    const float PLAYER_VIEW = 1.7f;

    bool[][][] collisions;

    Viewport minimapViewport;
    MinimapCamera minimapCamera;

    Viewport mainViewport;
    Camera mainCamera;

    int SCORE = 0;
    int TO_COLLECT = 0;

    List<Model>[] walls;
    List<Door>[] doors;
    List<Collectible>[] collectibles;
    List<Plane> planes;
    List<Ramp>[] ramps;
    List<Teleport>[] teleports;
    Dictionary<char, Teleport> telportPairs = new Dictionary<char, Teleport>();

    Light flashLight;
    bool grabbed;
    bool toogle;

    bool lightMode = false;

    int mapSizeX;
    int mapSizeZ;
    int floors_count;

    private Queue<double> frameTimes = new Queue<double>();
    private double totalFrameTimes = 0;
    private int currentFPS;


    private float headBobTimer = 0.0f;
    private float baseY = PLAYER_VIEW;
    private float baseY_k;

    public Program() : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize=new OpenTK.Mathematics.Vector2i(1366,768) }) { }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.5f, 0.7f, 1.0f, 1.0f);

        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);
        GL.DebugMessageCallback(DebugCallback, IntPtr.Zero);
        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Less);

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

        minimapViewport = new Viewport()
        {
            Left = 0.0,    // 75% from the left (right quarter of screen)
            Top = 0.0,      // Top of the screen
            Width = 0.25,   // 25% width of screen
            Height = 0.30,  // 25% height of screen
            window = this  // your instance of GameWindow
        };

        this.LoadMap("maps/advanced.md");
        flashLight = new Light(new Vector3(-mapSizeX / 2, -1, -mapSizeZ / 2), false, 0.1f);        
        minimapCamera.InitializePlayerMarker();
    }

    private static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
    {
        string msg = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(message, length);
        Console.WriteLine($"OpenGL Debug Message: {msg}\nSource: {source}, Type: {type}, Severity: {severity}, ID: {id}");
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        if(mainViewport != null) mainViewport.Set();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        updateFPS(args.Time);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // --- MAIN VIEWPORT ---
        mainViewport.Set();
        mainViewport.Clear();

        if (lightMode)
        {
            // --- PASS 1: Solid fill, slightly offset camera ---
            var view = mainCamera.View;
            var direction = new Vector3(view[0, 2], view[1, 2], view[2, 2]);
            direction = direction / new Vector3(
                MathF.Abs(direction.X) > 0 ? MathF.Abs(direction.X) : 1,
                MathF.Abs(direction.Y) > 0 ? MathF.Abs(direction.Y) : 1,
                MathF.Abs(direction.Z) > 0 ? MathF.Abs(direction.Z) : 1
            ) * 0.01f;

            mainCamera.pos += direction;

            // Draw filled scene (lightMode = false)
            renderScene(false);
            GL.Clear(ClearBufferMask.ColorBufferBit); // clear color but keep depth

            mainCamera.pos -= direction;

            // --- PASS 2: Wireframe, original camera position ---
            renderScene(true);
        }
        else
        {
            renderScene(false);
        }

        // --- MINIMAP VIEWPORT ---
        renderMinimap();

        SwapBuffers();
    }

    public void renderMinimap()
    {
        minimapCamera.pos = mainCamera.pos;
        minimapViewport.Set();
        minimapViewport.Clear();
        if (ramps[mainCamera.k].Count > 0) foreach (var ramp in ramps[mainCamera.k]) ramp.Draw(minimapCamera, flashLight, toogle, false, true);
        planes[mainCamera.k].Draw(minimapCamera, flashLight, toogle, false, true);
        if (walls[mainCamera.k].Count > 0) foreach(var wall in walls[mainCamera.k]) wall.Draw(minimapCamera, flashLight, toogle, false, true);
        if (doors[mainCamera.k].Count > 0) foreach(var door in doors[mainCamera.k]) door.Draw(minimapCamera, flashLight, toogle, false, true);
        if (collectibles[mainCamera.k].Count > 0) foreach(var item in collectibles[mainCamera.k]) item.Draw(minimapCamera, flashLight, toogle, false, true);
        if (teleports[mainCamera.k].Count > 0) foreach(var teleport in teleports[mainCamera.k]) teleport.Draw(minimapCamera, flashLight, toogle, false, true);
        minimapCamera.DrawPlayerMarker(mainCamera.pos);
    }

    public void renderScene(bool lightMode)
    {
        for (int i = mainCamera.k - 1; i <= mainCamera.k + 1; i++)
        {
            if (i >= 0 && i < floors_count)
            {
                planes[i].Draw(mainCamera, flashLight, toogle, lightMode);
                if (ramps[i].Count > 0) foreach (var ramp in ramps[i]) ramp.Draw(mainCamera, flashLight, toogle, lightMode);
                if (walls[i].Count> 0) foreach (var wall in walls[i]) wall.Draw(mainCamera, flashLight, toogle, lightMode);
                if (doors[i].Count > 0) foreach (var door in doors[i]) door.Draw(mainCamera, flashLight, toogle, lightMode);
                if (collectibles[i].Count > 0) foreach (var item in collectibles[i]) item.Draw(mainCamera, flashLight, toogle, lightMode);
                if (teleports[i].Count > 0)  foreach (var teleport in teleports[i]) teleport.Draw(mainCamera, flashLight, toogle, lightMode);
            }
            else if (i == floors_count)
            {
                planes[i].Draw(mainCamera, flashLight, toogle, lightMode);
            }
        }

        if (mainCamera.k + 2 <= floors_count)
        {
            planes[mainCamera.k + 2].Draw(mainCamera, flashLight, toogle, lightMode);
        }
     }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        float deltaTime = (float)e.Time;
        Vector2 direction = new Vector2(0, 0);

        // End game when all collectibles are collected
        if (SCORE >= TO_COLLECT)
        {
            Console.WriteLine("Congratulations! You collected all items!");
            Close();
            return;
        }
        // Animations
        foreach (var door in doors[mainCamera.k])
        {
            door.Update(deltaTime);
        }
        foreach (var item in collectibles[mainCamera.k])
        {
            item.Animation((float)deltaTime);
        }
        foreach (var teleport in teleports[mainCamera.k])
        {
            teleport.Animation((float)deltaTime);
        }

        // Keyboard input
        if (KeyboardState.IsKeyDown(Keys.Escape)) Close();
        if (KeyboardState.IsKeyDown(Keys.W)) direction += Vector2.UnitY;
        if (KeyboardState.IsKeyDown(Keys.S)) direction -= Vector2.UnitY;
        if (KeyboardState.IsKeyDown(Keys.A)) direction -= Vector2.UnitX;
        if (KeyboardState.IsKeyDown(Keys.D)) direction += Vector2.UnitX;
        if (KeyboardState.IsKeyPressed(Keys.F)) toogle = !toogle;
        if (KeyboardState.IsKeyPressed(Keys.E))
        {
            foreach (var door in doors[mainCamera.k])
            {
                if (door.Check(mainCamera, collisions))
                {
                    door.Toggle();
                }
            }

            foreach (var collectible in collectibles[mainCamera.k])
            {
                if (collectible.Check(mainCamera))
                {
                    SCORE += collectible.Collect();
                    collectibles[mainCamera.k].Remove(collectible);
                    break;
                }
            }
        }
        if (KeyboardState.IsKeyPressed(Keys.L))
        {
            if (lightMode) lightMode = false;
            else lightMode = true;
        }

        // Camera movement + ramp control
        var len = direction.Length;
        Vector3 previousPosition = mainCamera.pos;

        baseY_k = baseY + (3.1f * mainCamera.k);
        if (len > 0)
        {
            direction *= deltaTime / len; // Using the deltaTime we got from e.Time
            mainCamera.Move(direction.X, direction.Y, collisions);

            headBobTimer += deltaTime * 6.0f; // Add this as a class field
            float newCamY = (float)Math.Sin(headBobTimer) * 0.05f;
            mainCamera.pos.Y = baseY_k + newCamY;
            if (ramps != null)
            {
                var rampK = ramps[mainCamera.k];
                foreach (var t in rampK) t.isOnRamp(mainCamera, previousPosition);
                if (mainCamera.k > 0)
                {
                    rampK = ramps[mainCamera.k - 1];
                    foreach (var t in rampK) t.isOnRamp(mainCamera, previousPosition);
                }
            }
        }


        // Camera rotation
        if (mouseDelta.LengthSquared > 0.00001)
        {
            mainCamera.RotateX(mouseDelta.Y * deltaTime);
            mainCamera.RotateY(mouseDelta.X * deltaTime);
            minimapCamera.up = mainCamera.Front;
            mouseDelta = Vector2.Zero;
        }

        // Teleportation
        if (teleports[mainCamera.k].Count > 0) { 
        foreach (var teleport in teleports[mainCamera.k]) teleport.Teleporting(mainCamera, deltaTime);
        mainCamera.whiteFade = teleports[mainCamera.k].Max(t => t.GetWhiteFadeIntensity());
        }
    }

    private void updateFPS(double elapsedTime)
    {
        frameTimes.Enqueue(elapsedTime);
        totalFrameTimes += elapsedTime;

        while (totalFrameTimes > 1.0)
        {
            totalFrameTimes -= frameTimes.Dequeue();
        }

        currentFPS = frameTimes.Count;
        Title = $"3D Maze   |   FPS: {currentFPS}   |   SCORE: {SCORE} / {TO_COLLECT}";
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
        int z = 0;
        int k = 0;
        using (StreamReader reader = new StreamReader(filename))
        {
            string line = reader.ReadLine();
            string[] mapsSize = line.Split('x');


            mapSizeX = int.Parse(mapsSize[0]) * 2;
            mapSizeZ = int.Parse(mapsSize[1]) * 2;
            if(mapsSize.Length > 2)
            {
                floors_count = int.Parse(mapsSize[2]);
            }
            else
            {
                floors_count = 1;
            }
            initModelBuffers();

            // <= for rooftop
            planes.Add(new Plane(new Vector3(mapSizeX / 2, -0.05f, mapSizeZ / 2), mapSizeX, mapSizeZ));
            planes[^1].Shader = new Shader("shaders/texture.vert", "shaders/texture.frag");
            planes[^1].Material = new Material() { specular = new Vector3(0.2f, 0.2f, 0.2f), shininess = 8, textured = true };
            planes[^1].Textures.Add("diffuseTex", new Texture("textures/Brick_02.png"));
            
            collisions = new bool[mapSizeZ / 2][][];
            for (int i = 0; i < mapSizeZ / 2; i++)
            {
                collisions[i] = new bool[mapSizeX / 2][];

                for (int j = 0; j < mapSizeX / 2; j++)
                {
                    collisions[i][j] = new bool[floors_count];
                }
            }

            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length == 0 || line[0] == '#')
                {
                    k++;
                    if (k == floors_count) break;
                    z = 0;
                    continue;
                }
                for (int x = 0; x < mapSizeX/2; x++)
                {
                    char cell = line[x];
                    Vector3 position = new Vector3((x * 2) + 1, (float)(3.1 * k), (z * 2) + 1);
                    
                    if (cell == ' ')
                    {
                        collisions[z][x][k] = false;
                    }
                    else if (cell == '@')
                    {
                        Vector3 spawnPosition = new Vector3(position.X, PLAYER_VIEW + (float)(3.1f * k), position.Z);
                        mainCamera = new Camera(mainViewport);
                        mainCamera.k = k;
                        mainCamera.pos = spawnPosition;
                        minimapCamera = new MinimapCamera(minimapViewport);
                        minimapCamera.pos = spawnPosition;
                        collisions[z][x][k] = false;
                    }
                    else if (cell >= 'o' && cell <= 'z')
                    {
                        walls[k].Add(new Wall(position));
                        walls[k][^1].Shader = new Shader("shaders/texture.vert", "shaders/texture.frag");
                        walls[k][^1].Material = new Material() {specular = new Vector3(0.2f, 0.2f, 0.2f), shininess = 8, textured = true };
                        walls[k][^1].Textures.Add("diffuseTex", new Texture("textures/brick_12-512x512.png"));

                        collisions[z][x][k] = true;
                    }
                    else if (cell >= 'A' && cell <= 'G')
                    {
                        doors[k].Add(new Door(position, x, z, k, collisions));
                        doors[k][^1].Shader = new Shader("shaders/texture.vert", "shaders/texture.frag");
                        doors[k][^1].Material = new Material() { specular = new Vector3(0.4f, 0.4f, 0.4f), shininess = 8, textured = true };
                        doors[k][^1].Textures.Add("diffuseTex", new Texture("textures/cratetex.png"));
                        collisions[z][x][k] = true;
                    }
                    else if (cell == 'R')
                    {
                        collisions[z][x][k] = false;
                        if (k == floors_count - 1) continue;
                        ramps[k].Add(new Ramp(position, k+1));
                        ramps[k][^1].Shader = new Shader("shaders/texture.vert", "shaders/texture.frag");
                        ramps[k][^1].Material = new Material() { specular = new Vector3(0.4f, 0.4f, 0.4f), shininess = 8, textured = true };
                        ramps[k][^1].Textures.Add("diffuseTex", new Texture("textures/brick_12-512x512.png"));


                        planes.Add(new Plane(new Vector3(mapSizeX / 2, -0.05f + (3.1f * (1+k)), mapSizeZ / 2), mapSizeX, mapSizeZ, x, z));
                        planes[^1].Shader = new Shader("shaders/texture.vert", "shaders/texture.frag");
                        planes[^1].Material = new Material() { specular = new Vector3(0.2f, 0.2f, 0.2f), shininess = 8, textured = true };
                        planes[^1].Textures.Add("diffuseTex", new Texture("textures/Brick_02.png"));
                    }
                    else if (cell >= 'T' && cell <= 'Z')
                    {
                        Vector3 specialPosition = position;
                        specialPosition.Y = 3.10f * k;
                        collectibles[k].Add(new Collectible("assets/coin_bag.obj", specialPosition));
                        collectibles[k][^1].Shader = new Shader("shaders/texture.vert", "shaders/texture.frag");
                        collectibles[k][^1].Material = new Material() { diffuse = new Vector3(0.7f, 0.5f, 0.0f), specular = new Vector3(0.8f, 0.8f, 0.1f), shininess = 50, textured = true };
                        collectibles[k][^1].Textures.Add("diffuseTex", new Texture("textures/coin_bag_texture.jpg"));

                        TO_COLLECT++;
                        collisions[z][x][k] = false;
                    }
                    else if (cell >= '1' && cell <= '9')
                    {
                        teleports[k].Add(new Teleport(position, (int)cell));
                        teleports[k][^1].Shader = new Shader("shaders/texture.vert", "shaders/texture.frag");
                        teleports[k][^1].Material = new Material() { specular = new Vector3(0.4f, 0.4f, 0.4f), shininess = 8, textured = true };
                        teleports[k][^1].Textures.Add("diffuseTex", new Texture("textures/teleport-texture.png"));

                        if (!telportPairs.ContainsKey(cell))
                        {
                            telportPairs.Add(cell, teleports[k][^1]);
                        }
                        else
                        {
                            teleports[k][^1].pair = telportPairs[cell];
                            telportPairs[cell].pair = teleports[k][^1];
                            telportPairs.Remove(cell);
                        }
                        collisions[z][x][k] = false;
                    }
                }
                z++;
            }
        }
        planes.Add(new Plane(new Vector3(mapSizeX / 2, 3.1f * ((k+1)), mapSizeZ / 2), mapSizeX, mapSizeZ));
        planes[^1].Shader = new Shader("shaders/texture.vert", "shaders/texture.frag");
        planes[^1].Material = new Material() { specular = new Vector3(0.2f, 0.2f, 0.2f), shininess = 8, textured = true };
        planes[^1].Textures.Add("diffuseTex", new Texture("textures/Brick_02.png"));
        return;
    }

    public void initModelBuffers()
    {
        walls = new List<Model>[floors_count];
        doors = new List<Door>[floors_count];
        collectibles = new List<Collectible>[floors_count];
        planes = new List<Plane>();
        ramps = new List<Ramp>[floors_count];
        teleports = new List<Teleport>[floors_count];
        for (int i = 0; i < floors_count; i++)
        {
            walls[i] = new List<Model>();
            doors[i] = new List<Door>();
            collectibles[i] = new List<Collectible>();
            ramps[i] = new List<Ramp>();
            teleports[i] = new List<Teleport>();
        }
    }

    public override void Close()
    {
        base.Close();
        Console.WriteLine(@"
         _____ _                 _                 _           
        |_   _| |               | |               (_)          
          | | | |__   __ _ _ __ | | _____ _ __ ___ _  ___  ___ 
          | | | '_ \ / _` | '_ \| |/ / _ \ '__/ __| |/ _ \/ __|
          | | | | | | (_| | | | |   <  __/ |  \__ \ |  __/\__ \
          \_/ |_| |_|\__,_|_| |_|_|\_\___|_|  |___/_|\___||___/
                                                      
                     Thanks for playing my game!
        ");
    }
}
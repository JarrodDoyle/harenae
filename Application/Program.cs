using Application.UI;
using Raylib_cs;
using ImGuiNET;
using Simulation;
using System.Numerics;

namespace Application;

internal static class Program
{
    private static readonly Color[] Colors = { Color.BLACK, Color.BEIGE, Color.SKYBLUE, Color.LIGHTGRAY, Color.DARKGRAY, };

    private static void InitWindow(int width, int height, string title)
    {
        Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT | ConfigFlags.FLAG_VSYNC_HINT |
                              ConfigFlags.FLAG_WINDOW_RESIZABLE);
        Raylib.SetTraceLogLevel(TraceLogLevel.LOG_WARNING);
        Raylib.InitWindow(width, height, title);
        Raylib.SetWindowMinSize(640, 480);
    }

    private static void Main()
    {
        InitWindow(1280, 720, "Haranae Editor");

        ElementRegistry.RegisterElement("Empty", new ElementSettings(false, false, false, false, new string[] { }, new Vector4(0, 0, 0, 255)));
        ElementRegistry.RegisterElement("Sand", new ElementSettings(false, false, true, true, new string[]{"Sand", "Stone"}, new Vector4(211, 176, 131, 255)));
        ElementRegistry.RegisterElement("Water", new ElementSettings(false, true, true, true, new string[]{"Water", "Sand", "Stone"}, new Vector4(102, 191, 255, 255)));
        ElementRegistry.RegisterElement("Smoke", new ElementSettings(true, true, true, true, new string[]{"Smoke", "Sand", "Water", "Stone"}, new Vector4(200, 200, 200, 255)));
        ElementRegistry.RegisterElement("Stone", new ElementSettings(false, false, false, false, new string[]{}, new Vector4(80, 80, 80, 255)));

        var world = new World(5, 3, 64);
        BrushManager.Setup();
        UiManager.Setup();

        var rnd = new Random();

        while (!Raylib.WindowShouldClose())
        {
            UiManager.Update();

            world.Step();
            HandleInput(world);

            foreach (var particlePosition in world.UpdatedParticles)
            {
                var x = (int)particlePosition.X;
                var y = (int)particlePosition.Y;
                var element = world.GetElement(x, y);
                if (element == null) continue;

                var bc = element.Settings.BaseColour;
                var color = new Color((int)bc.X, (int)bc.Y, (int)bc.Z, (int)bc.W);
                if (element.Name != "Empty")
                {
                    var noise = rnd.Next(-10, 11);
                    color.r = (byte) Math.Clamp(color.r + noise, 0, 255);
                    color.g = (byte) Math.Clamp(color.g + noise, 0, 255);
                    color.b = (byte) Math.Clamp(color.b + noise, 0, 255);
                }
                SimulationRenderer.DrawPixel(x, y, color);
            }

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RAYWHITE);

            SimulationRenderer.Render();
            UiManager.Render();

            Raylib.DrawFPS(0, 0);
            Raylib.EndDrawing();
        }

        ImGuiBackend.Shutdown();
        Raylib.CloseWindow();
    }

    private static void HandleInput(World world)
    {
        var io = ImGui.GetIO();
        if (!io.WantCaptureMouse)
        {
            if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
            {
                var pos = SimulationRenderer.ScreenToWorld(Raylib.GetMousePosition());
                BrushManager.DrawBrush(world, (int) pos.X, (int) pos.Y);
            }

            if (Raylib.GetMouseWheelMove() < 0) BrushManager.BrushSize -= 1;
            if (Raylib.GetMouseWheelMove() > 0) BrushManager.BrushSize += 1;
        }
    
        if (!io.WantCaptureKeyboard)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ONE)) BrushManager.Element = "Empty";
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_TWO)) BrushManager.Element = "Sand";
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_THREE)) BrushManager.Element = "Water";
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_FOUR)) BrushManager.Element = "Smoke";
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_FIVE)) BrushManager.Element = "Stone";

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE)) world.Redraw();
        }  
    }
}
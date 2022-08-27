using Application.UI;
using Raylib_cs;
using ImGuiNET;
using Simulation;

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
        InitWindow(1280, 720, "Raylib + Dear ImGui app");

        var world = new World(320, 180);
        SimulationRenderer.EnqueueAction(() => Raylib.ClearBackground(Color.BLACK));

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

                var color = Colors[(int)element];
                if (element != ElementType.Empty)
                {
                    var noise = rnd.Next(-10, 11);
                    color.r = (byte) Math.Clamp(color.r + noise, 0, 255);
                    color.g = (byte) Math.Clamp(color.g + noise, 0, 255);
                    color.b = (byte) Math.Clamp(color.b + noise, 0, 255);
                }
                SimulationRenderer.EnqueueAction(() => Raylib.DrawPixel(x, y, color));
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
                var xScale = Raylib.GetScreenWidth() / 320.0f;
                var yScale = Raylib.GetScreenHeight() / 180.0f;
                var scale = (int)Math.Floor(Math.Min(xScale, yScale));
                var mousePos = Raylib.GetMousePosition() / scale;
                BrushManager.DrawBrush(world, (int)mousePos.X, (int)mousePos.Y);
            }

            if (Raylib.GetMouseWheelMove() < 0) BrushManager.BrushSize = Math.Max(1, BrushManager.BrushSize - 1);
            if (Raylib.GetMouseWheelMove() > 0) BrushManager.BrushSize = Math.Min(20, BrushManager.BrushSize + 1);
        }
    
        if (!io.WantCaptureKeyboard)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ONE)) BrushManager.Element = ElementType.Empty;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_TWO)) BrushManager.Element = ElementType.Sand;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_THREE)) BrushManager.Element = ElementType.Water;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_FOUR)) BrushManager.Element = ElementType.Smoke;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_FIVE)) BrushManager.Element = ElementType.Stone;

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE)) world.Redraw();
        }  
    }
}
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

        ImGuiController.Setup();
        var uiPanels = new List<Panel>();
        uiPanels.Add(new BrushSettingsPanel { Open = true });
        foreach (Panel panel in uiPanels)
            panel.Attach();

        var world = new World(320, 180);
        SimulationRenderer.EnqueueAction(() => Raylib.ClearBackground(Color.BLACK));

        BrushManager.Setup();

        while (!Raylib.WindowShouldClose())
        {
            foreach (Panel panel in uiPanels)
                panel.Update();

            world.Step();

            if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
            {
                var scale = (int) Math.Floor(Math.Min(
                    Raylib.GetScreenWidth() / 320.0f, Raylib.GetScreenHeight() / 180.0f));
                var mousePos = Raylib.GetMousePosition() / scale;

                BrushManager.DrawBrush(world, (int)mousePos.X, (int)mousePos.Y);
            }

            if (Raylib.GetMouseWheelMove() < 0) BrushManager.BrushSize = Math.Max(1, BrushManager.BrushSize - 1);
            if (Raylib.GetMouseWheelMove() > 0) BrushManager.BrushSize = Math.Min(20, BrushManager.BrushSize + 1);
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ONE)) BrushManager.Element = ElementType.Empty;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_TWO)) BrushManager.Element = ElementType.Sand;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_THREE)) BrushManager.Element = ElementType.Water;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_FOUR)) BrushManager.Element = ElementType.Smoke;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_FIVE)) BrushManager.Element = ElementType.Stone;

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE)) world.Redraw();


            foreach (var particlePosition in world.UpdatedParticles)
            {
                var x = (int)particlePosition.X;
                var y = (int)particlePosition.Y;
                var element = world.GetElement(x, y);
                if (element == null) continue;
                var color = Colors[(int)element];
                SimulationRenderer.EnqueueAction(() => Raylib.DrawPixel(x, y, color));
            }

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RAYWHITE);

            SimulationRenderer.Render();

            ImGuiController.Begin();
            ImGui.DockSpaceOverViewport(ImGui.GetMainViewport(), ImGuiDockNodeFlags.PassthruCentralNode);
            foreach (Panel panel in uiPanels)
                panel.Render();
            ImGuiController.End();

            Raylib.DrawFPS(0, 0);
            Raylib.EndDrawing();
        }

        ImGuiController.Shutdown();
        Raylib.CloseWindow();
    }
}
using Application.UI;
using Raylib_cs;
using ImGuiNET;

namespace Application;

internal static class Program
{
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
        var uiLayers = new List<BaseUiLayer>();
        foreach (BaseUiLayer layer in uiLayers)
            layer.Attach();

        var particleType = ParticleType.Sand;
        var world = new World(320, 180, 144);
        SimulationRenderer.EnqueueAction(() => Raylib.ClearBackground(Color.BLACK));

        while (!Raylib.WindowShouldClose())
        {
            foreach (BaseUiLayer layer in uiLayers)
                layer.Update();

            if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
            {
                var scale = (int) Math.Floor(Math.Min(Raylib.GetScreenWidth() / 320.0f,
                    Raylib.GetScreenHeight() / 180.0f));
                var mousePos = Raylib.GetMousePosition() / scale;
                world.SetParticle((int) mousePos.X, (int) mousePos.Y, particleType);
            }

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ONE)) particleType = ParticleType.Air;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_TWO)) particleType = ParticleType.Sand;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_THREE)) particleType = ParticleType.Water;

            world.Update();

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RAYWHITE);

            SimulationRenderer.Render();

            ImGuiController.Begin();
            ImGui.DockSpaceOverViewport(ImGui.GetMainViewport(), ImGuiDockNodeFlags.PassthruCentralNode);
            foreach (BaseUiLayer layer in uiLayers)
                layer.Render();
            ImGuiController.End();

            Raylib.DrawFPS(0, 0);
            Raylib.EndDrawing();
        }

        ImGuiController.Shutdown();
        Raylib.CloseWindow();
    }
}
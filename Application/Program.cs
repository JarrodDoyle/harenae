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
        var uiLayers = new List<BaseUiLayer> {new ExampleLayer {Open = true}};
        foreach (BaseUiLayer layer in uiLayers)
            layer.Attach();

        while (!Raylib.WindowShouldClose())
        {
            foreach (BaseUiLayer layer in uiLayers)
                layer.Update();

            Raylib.BeginDrawing();
            
            Raylib.ClearBackground(Color.RAYWHITE);
            Raylib.DrawFPS(0, 0);

            ImGuiController.Begin();
            ImGui.DockSpaceOverViewport(ImGui.GetMainViewport(), ImGuiDockNodeFlags.PassthruCentralNode);
            ImGui.ShowDemoWindow();
            foreach (BaseUiLayer layer in uiLayers)
                layer.Render();
            ImGuiController.End();

            Raylib.EndDrawing();
        }

        ImGuiController.Shutdown();
        Raylib.CloseWindow();
    }
}
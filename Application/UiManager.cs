using ImGuiNET;

namespace Application;

public static class UiManager
{
    public static List<UI.Panel> Panels { get; set; } = new List<UI.Panel>();


    public static void Setup()
    {
        UI.ImGuiBackend.Setup();

        Panels.Clear();
        Panels.Add(new UI.BrushSettingsPanel());
        foreach (var panel in Panels)
            panel.Attach();
    }

    public static void Update()
    {
        foreach (var panel in Panels)
            panel.Update();
    }

    public static void Render()
    {
        UI.ImGuiBackend.Begin();
        ImGui.DockSpaceOverViewport(ImGui.GetMainViewport(), ImGuiDockNodeFlags.PassthruCentralNode);
        ImGui.ShowDemoWindow();
        foreach (var panel in Panels)
            panel.Render();
        UI.ImGuiBackend.End();
    }
}
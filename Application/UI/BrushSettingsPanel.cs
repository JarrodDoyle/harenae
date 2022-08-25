using ImGuiNET;

namespace Application.UI;

public class BrushSettingsPanel : Panel
{
    public override void Attach()
    {
    }

    public override void Detach()
    {
    }

    public override void Render()
    {
        var open = Open;
        if (!open) return;

        if (ImGui.Begin("Brush Settings", ref open))
        {
            ImGui.End();
        }

        Open = open;
    }

    public override void Update()
    {
    }
}
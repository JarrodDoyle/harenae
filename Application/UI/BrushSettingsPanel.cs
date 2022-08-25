using ImGuiNET;
using Simulation;

namespace Application.UI;

public class BrushSettingsPanel : Panel
{
    public override void Attach()
    {
        Open = true;
    }

    public override void Detach()
    {
        Open = false;
    }

    public override void Render()
    {
        var open = Open;
        if (!open) return;

        if (ImGui.Begin("Brush Settings", ref open))
        {
            {
                string[] items = Enum.GetNames(typeof(Simulation.ElementType));
                var current = (int) BrushManager.Element;
                if (ImGui.Combo("Element", ref current, items, items.Length))
                    BrushManager.Element = (Simulation.ElementType)current;
            }
            
            {
                string[] items = Enum.GetNames(typeof(BrushShape));
                var current = (int) BrushManager.BrushShape;
                if (ImGui.Combo("Shape", ref current, items, items.Length))
                    BrushManager.BrushShape = (BrushShape) current;
            }

            {
                // TODO: Try working out how to use InputScalar here
                var size = (int) BrushManager.BrushSize;
                if (ImGui.InputInt("Size", ref size))
                    BrushManager.BrushSize = (uint) Math.Clamp(size, 1, 20);
            }

            ImGui.End();
        }

        Open = open;
    }

    public override void Update()
    {
    }
}
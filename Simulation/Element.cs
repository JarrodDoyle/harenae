using System.Numerics;

namespace Simulation;

public class Element
{
    public string Name;
    public ElementSettings Settings { get; }
    private (int, int)[] MoveDirections { get; }

    public Element(string name, ElementSettings settings)
    {
        Name = name;
        Settings = settings;

        var moveDirections = new List<(int, int)>();
        var dy = Settings.Rise ? -1 : 1;
        if (Settings.MoveY) moveDirections.Add((0, dy));
        if (Settings.MoveXY)
        {
            moveDirections.Add((-1, dy));
            moveDirections.Add((1, dy));
        }
        if (Settings.MoveX)
        {
            moveDirections.Add((-1, 0));
            moveDirections.Add((1, 0));
        }

        MoveDirections = moveDirections.ToArray();
    }

    public void Step(World world, int x, int y, bool flippedX, bool rise)
    {
        if (Settings.Rise != rise) return;

        foreach (var (dx, dy) in MoveDirections)
        {
            var x2 = x + (flippedX ? -dx : dx);
            var y2 = y + dy;
            var other = world.GetElement(x2, y2);
            if (other == null || Settings.Blockers.Contains(other.Name)) continue;
            world.SwapElements(x, y, x2, y2);
        }
    }
}
namespace Simulation;

public enum ElementType
{
    Empty,
    Sand,
    Water,
    Smoke,
    Stone
}

public class Element
{
    private (int, int)[] MoveDirections { get; }
    private ElementType[] MoveBlockers { get; }

    private Element((int, int)[] moveDirections, ElementType[] moveBlockers)
    {
        MoveDirections = moveDirections;
        MoveBlockers = moveBlockers;
    }

    public static void Step(World world, ElementType elementType, int x, int y, bool flippedX)
    {
        var element = Elements[(int) elementType];
        foreach (var (dx, dy) in element.MoveDirections)
        {
            var x2 = x + (flippedX ? -dx : dx);
            var y2 = y + dy;
            var other = world.GetElement(x2, y2);
            if (other == null || element.MoveBlockers.Contains((ElementType) other)) continue;
            world.SwapElements(x, y, x2, y2);
        }
    }

    private static readonly Element[] Elements =
    {
        new(Array.Empty<(int, int)>(),
            Array.Empty<ElementType>()
        ), // Empty
        new(new[] {(0, 1), (-1, 1), (1, 1)},
            new[] {ElementType.Sand, ElementType.Stone}
        ), // Sand
        new(new[] {(0, 1), (-1, 1), (1, 1), (-1, 0), (1, 0)},
            new[] {ElementType.Water, ElementType.Sand, ElementType.Stone}
        ), // Water
        new(new[] {(0, -1), (-1, -1), (1, -1), (-1, 0), (1, 0)},
            new[] {ElementType.Smoke, ElementType.Sand, ElementType.Water, ElementType.Stone}
        ), // Smoke
        new(Array.Empty<(int, int)>(),
            Array.Empty<ElementType>()
        ), // Stone
    };
}
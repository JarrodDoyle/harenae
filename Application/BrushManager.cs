using Simulation;

namespace Application;

public enum BrushShape
{
    Square,
    Circle,
}

public static class BrushManager
{
    public static string Element { get; set; } = string.Empty;
    private static uint _brushSize;
    public static uint BrushSize
    {
        get => _brushSize;
        set => _brushSize = Math.Clamp(value, 1, 30);
    }
    public static BrushShape BrushShape { get; set; }


    public static void Setup()
    {
        Element = "Sand";
        BrushSize = 1;
        BrushShape = BrushShape.Square;
    }

    public static void DrawBrush(World world, int x, int y)
    {
        var halfRadius = (int)BrushSize / 2;
        x -= halfRadius;
        y -= halfRadius;

        switch (BrushShape)
        {
            case BrushShape.Square:
                DrawSquare(world, x, y);
                break;
            case BrushShape.Circle:
                break;
        }
    }

    private static void DrawSquare(World world, int x, int y)
    {
        for (var dx = 0; dx < BrushSize; dx++)
        {
            for (var dy = 0; dy < BrushSize; dy++)
            {
                world.SetElement(x + dx, y + dy, Element);
            }
        }
    }
}
using System.Numerics;

namespace Simulation;

public class World
{
    public HashSet<Vector2> UpdatedParticles { get; }
    private readonly int _width;
    private readonly int _height;
    private ElementType[,] _elements;
    private ElementType[,] _newElements;

    public World(int width, int height)
    {
        UpdatedParticles = new HashSet<Vector2>();
        _width = width;
        _height = height;
        _elements = new ElementType[_width, _height];
        for (var x = 0; x < _width; x++)
        for (var y = 0; y < _height; y++)
            _elements[x, y] = ElementType.Empty;
        _newElements = (ElementType[,]) _elements.Clone();
    }

    public void Step()
    {
        UpdatedParticles.Clear();
        _newElements = (ElementType[,]) _elements.Clone();

        for (var y = _height - 1; y >= 0; y--)
        {
            if (y % 2 == 0)
                for (var x = 0; x < _width; x++)
                    Element.Step(this, _elements[x, y], x, y, true);
            else
                for (var x = _width - 1; x >= 0; x--)
                    Element.Step(this, _elements[x, y], x, y, false);
        }

        _elements = _newElements;
    }

    public void SwapElements(int x1, int y1, int x2, int y2)
    {
        (_newElements[x1, y1], _newElements[x2, y2]) = (_newElements[x2, y2], _newElements[x1, y1]);

        UpdatedParticles.Add(new Vector2(x1, y1));
        UpdatedParticles.Add(new Vector2(x2, y2));
    }

    public ElementType? GetElement(int x, int y)
    {
        return PosInWorld(x, y) ? _newElements[x, y] : null;
    }

    public void SetElement(int x, int y, ElementType elementType)
    {
        if (!PosInWorld(x, y)) return;
        _elements[x, y] = elementType;
        UpdatedParticles.Add(new Vector2(x, y));
    }

    public void Redraw()
    {
        for (var x = 0; x < _width; x++)
        for (var y = 0; y < _height; y++)
            UpdatedParticles.Add(new Vector2(x, y));
    }

    private bool PosInWorld(int x, int y)
    {
        return x >= 0 && x < _width && y >= 0 && y < _height;
    }
}
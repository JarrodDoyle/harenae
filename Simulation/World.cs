using System.Numerics;

namespace Simulation;

public class World
{
    public HashSet<Vector2> UpdatedParticles { get; }
    private readonly int _width;
    private readonly int _height;
    private readonly Element[,] _elements;
    private readonly Random _rnd;

    public World(int width, int height)
    {
        UpdatedParticles = new HashSet<Vector2>();
        _rnd = new Random();
        _width = width;
        _height = height;
        _elements = new Element[_width, _height];
        for (var x = 0; x < _width; x++)
        for (var y = 0; y < _height; y++)
            _elements[x, y] = ElementRegistry.GetElement("Empty");
    }

    public void Step()
    {
        UpdatedParticles.Clear();

        // Randomise the order of X updates to give a more natural feel for fluid-like element dispersal
        var xIndices = new int[_width];
        for (var i = 0; i < _width; i++)
        {
            xIndices[i] = i;
        }

        for (var i = 0; i < _width - 1; i++)
        {
            var j = i + _rnd.Next(_width - i);
            (xIndices[j], xIndices[i]) = (xIndices[i], xIndices[j]);
        }

        // Update falling particles bottom up, then update rising particles top down
        for (var y = _height - 1; y >= 0; y--)
            foreach (var x in xIndices)
                _elements[x, y].Step(this, x, y, _rnd.Next(0, 2) == 1, false);

        for (var y = 0; y < _height; y++)
            foreach (var x in xIndices)
                _elements[x, y].Step(this, x, y, _rnd.Next(0, 2) == 1, true);
    }

    public void SwapElements(int x1, int y1, int x2, int y2)
    {
        (_elements[x1, y1], _elements[x2, y2]) = (_elements[x2, y2], _elements[x1, y1]);

        UpdatedParticles.Add(new Vector2(x1, y1));
        UpdatedParticles.Add(new Vector2(x2, y2));
    }

    public Element? GetElement(int x, int y)
    {
        return PosInWorld(x, y) ? _elements[x, y] : null;
    }

    public void SetElement(int x, int y, string elementType)
    {
        if (!PosInWorld(x, y)) return;
        var element = ElementRegistry.GetElement(elementType);
        _elements[x, y] = element;
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
using System.Numerics;

namespace Simulation;

public class World
{
    public HashSet<Vector2> UpdatedParticles { get; } = new();
    private readonly int _width;
    private readonly int _height;
    private readonly Element[,] _elements;
    private readonly Random _rnd;
    private readonly int[] _xIndices;
    private Vector2 _dirtyMin = Vector2.Zero;
    private Vector2 _dirtyMax = Vector2.Zero;

    public World(int width, int height)
    {
        _rnd = new Random();
        _width = width;
        _height = height;
        _elements = new Element[_width, _height];
        for (var x = 0; x < _width; x++)
        for (var y = 0; y < _height; y++)
            _elements[x, y] = ElementRegistry.GetElement("Empty");
        _xIndices = new int[_width];
        for (var i = 0; i < _xIndices.Length; i++)
            _xIndices[i] = i;
    }

    public void Step()
    {
        if (UpdatedParticles.Count == 0) return;

        // Construct initial dirtyrect (oof performance?)
        _dirtyMin = UpdatedParticles.First() - Vector2.One;
        _dirtyMax = _dirtyMin + 2 * Vector2.One;
        foreach (var pos in UpdatedParticles)
            UpdateDirtyRect(pos);

        UpdatedParticles.Clear();

        // Randomise the order of X updates to give a more natural feel for fluid-like element dispersal
        for (var i = 0; i < _xIndices.Length - 1; i++)
        {
            var j = i + _rnd.Next(_xIndices.Length - i);
            (_xIndices[j], _xIndices[i]) = (_xIndices[i], _xIndices[j]);
        }

        // Update falling particles bottom up, then update rising particles top down
        for (var y = _height - 1; y >= 0; y--)
        {
            foreach (var x in _xIndices)
            {
                if (!PosInDirtyRect(x, y)) continue;
                _elements[x, y].Step(this, x, y, _rnd.Next(0, 2) == 1, false);
            }
        }

        for (var y = 0; y < _height; y++)
        {
            foreach (var x in _xIndices)
            {
                if (!PosInDirtyRect(x, y)) continue;
                _elements[x, y].Step(this, x, y, _rnd.Next(0, 2) == 1, true);
            }
        }
    }

    public void SwapElements(int x1, int y1, int x2, int y2)
    {
        (_elements[x1, y1], _elements[x2, y2]) = (_elements[x2, y2], _elements[x1, y1]);

        UpdatedParticles.Add(new Vector2(x1, y1));
        UpdatedParticles.Add(new Vector2(x2, y2));
        UpdateDirtyRect(x1, y1);
        UpdateDirtyRect(x2, y2);
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

    private void UpdateDirtyRect(int x, int y)
    {
        if (x <= _dirtyMin.X) _dirtyMin.X = x - 1;
        if (x >= _dirtyMax.X) _dirtyMax.X = x + 1;
        if (y <= _dirtyMin.Y) _dirtyMin.Y = y - 1;
        if (y >= _dirtyMax.Y) _dirtyMax.Y = y + 1;
    }

    private void UpdateDirtyRect(Vector2 pos)
    {
        UpdateDirtyRect((int) pos.X, (int) pos.Y);
    }

    private bool PosInDirtyRect(int x, int y)
    {
        return x >= _dirtyMin.X && x <= _dirtyMax.X && y >= _dirtyMin.Y && y <= _dirtyMax.Y;
    }
}
using System.Numerics;

namespace Simulation;

public class World
{
    public HashSet<Vector2> UpdatedParticles { get; } = new();
    private readonly Dictionary<Vector2, Chunk> _chunks = new();
    private readonly int _width;
    private readonly int _height;
    private readonly int _chunkSize;
    private readonly Random _rnd;
    private readonly int[] _xIndices;

    public World(int width, int height, int chunkSize)
    {
        _rnd = new Random();
        _width = width;
        _height = height;
        _chunkSize = chunkSize;
        for (var y = 0; y < _height; y++)
        for (var x = 0; x < _width; x++)
        {
            var pos = new Vector2(x, y);
            _chunks.Add(pos, new Chunk(_chunkSize, _chunkSize, pos));
        }

        _xIndices = new int[_chunkSize];
        for (var i = 0; i < _xIndices.Length; i++)
            _xIndices[i] = i;
    }

    public void Step()
    {
        // Randomise the order of X updates to give a more natural feel for fluid-like element dispersal
        for (var i = 0; i < _xIndices.Length - 1; i++)
        {
            var j = i + _rnd.Next(_xIndices.Length - i);
            (_xIndices[j], _xIndices[i]) = (_xIndices[i], _xIndices[j]);
        }

        // Update chunks
        UpdatedParticles.Clear();
        var tasks = new List<Task>();
        foreach (var (_, chunk) in _chunks)
        {
            tasks.Add(Task.Run(() => { chunk.Step(this, _xIndices); }));
        }

        Task.WaitAll(tasks.ToArray());

        // Gather updated particles outside of tasks because UpdatedParticles doesn't support concurrent access
        foreach (var (chunkPos, chunk) in _chunks)
        {
            var posOffset = chunkPos * _chunkSize;
            foreach (var pos in chunk.UpdatedParticles)
                UpdatedParticles.Add(pos + posOffset);
        }
    }

    public void SwapElements(int x1, int y1, int x2, int y2)
    {
        // Only swap elements within the same chunk for now
        var chunkPos = GetPositionOfChunk(x1, y1);
        if (chunkPos != GetPositionOfChunk(x2, y2)) return;

        if (_chunks.TryGetValue(chunkPos, out var chunk))
        {
            var posInChunk1 = GetPositionInChunk(x1, y1);
            var posInChunk2 = GetPositionInChunk(x2, y2);
            chunk.SwapElements(posInChunk1, posInChunk2);
        }
    }

    public Element? GetElement(int x, int y)
    {
        var posOfChunk = GetPositionOfChunk(x, y);
        var posInChunk = GetPositionInChunk(x, y);
        var chunkFound = _chunks.TryGetValue(posOfChunk, out var chunk);
        return chunkFound ? chunk!.GetElement(posInChunk) : null;
    }

    public void SetElement(int x, int y, string elementType)
    {
        var posOfChunk = GetPositionOfChunk(x, y);
        var posInChunk = GetPositionInChunk(x, y);
        var chunkFound = _chunks.TryGetValue(posOfChunk, out var chunk);
        if (chunkFound)
        {
            chunk!.SetElement(posInChunk, elementType);
            UpdatedParticles.Add(new Vector2(x, y));
        }
    }

    public void Redraw()
    {
        for (var x = 0; x < _width * _chunkSize; x++)
        for (var y = 0; y < _width * _chunkSize; y++)
            UpdatedParticles.Add(new Vector2(x, y));
    }

    public Vector2 GetPositionOfChunk(int x, int y)
    {
        var chunkX = MathF.Floor((float) x / _chunkSize);
        var chunkY = MathF.Floor((float) y / _chunkSize);
        return new Vector2(chunkX, chunkY);
    }

    public Vector2 GetPositionInChunk(int x, int y)
    {
        return new Vector2(x % _chunkSize, y % _chunkSize);
    }
}
using Raylib_cs;

namespace Application;

public class World
{
    private readonly int _width;
    private readonly int _height;
    private readonly int _tps;
    private float _time;
    private readonly Particle[,] _grid;

    public World(int width, int height, int tps)
    {
        _width = width;
        _height = height;
        _tps = tps;
        _time = 0;
        _grid = new Particle[width, height];
        for (var x = 0; x < _width; x++)
        for (var y = _height - 1; y >= 0; y--)
            _grid[x, y] = new Particle();
    }

    public void SetSand(int x, int y)
    {
        if (!PosInWorld(x, y)) return;
        _grid[x, y].Type = ParticleType.Sand;
        SimulationRenderer.EnqueueAction(() => Raylib.DrawPixel(x, y, Color.GOLD));
    }

    public void Update()
    {
        _time += Raylib.GetFrameTime();

        while (_time >= 1.0f / _tps)
        {
            for (var x = 0; x < _width; x++)
            for (var y = _height - 1; y >= 0; y--)
                _grid[x, y].Updated = false;

            for (var x = 0; x < _width; x++)
            for (var y = _height - 1; y >= 0; y--)
            {
                var cell = _grid[x, y];
                if (cell.Updated) continue;

                switch (cell.Type)
                {
                    case ParticleType.Air:
                        cell.Updated = true;
                        break;
                    case ParticleType.Sand:
                        if (GetParticle(x, y + 1) is {Type: ParticleType.Air})
                            MoveParticle(x, y, x, y + 1);
                        if (GetParticle(x - 1, y + 1) is {Type: ParticleType.Air})
                            MoveParticle(x, y, x - 1, y + 1);
                        if (GetParticle(x + 1, y + 1) is {Type: ParticleType.Air})
                            MoveParticle(x, y, x + 1, y + 1);
                        break;
                    default:
                        Console.WriteLine("Unknown cell type.");
                        break;
                }
            }

            _time -= 1.0f / _tps;
        }
    }

    private void MoveParticle(int x1, int y1, int x2, int y2)
    {
        if (!PosInWorld(x1, y1) || !PosInWorld(x2, y2)) return;

        var c2Type = _grid[x1, y1].Type;
        _grid[x1, y1].Type = ParticleType.Air;
        _grid[x2, y2].Type = c2Type;
        _grid[x1, y1].Updated = true;
        _grid[x2, y2].Updated = true;
        SimulationRenderer.EnqueueAction(() =>
        {
            Raylib.DrawPixel(x1, y1, Particle.Colors[(int) ParticleType.Air]);
            Raylib.DrawPixel(x2, y2, Particle.Colors[(int) c2Type]);
        });
    }


    private Particle? GetParticle(int x, int y)
    {
        return !PosInWorld(x, y) ? null : _grid[x, y];
    }

    private bool PosInWorld(int x, int y)
    {
        return x >= 0 && x < _width && y >= 0 && y < _height;
    }
}
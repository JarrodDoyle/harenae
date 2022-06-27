using Raylib_cs;

namespace Application;

public enum ParticleType
{
    Air,
    Sand,
}

public class Particle
{
    public ParticleType Type { get; set; }
    public bool Updated { get; set; }
    public static readonly Color[] Colors = {Color.BLACK, Color.GOLD};

    public Particle()
    {
        Type = ParticleType.Air;
        Updated = false;
    }
}
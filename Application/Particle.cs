using Raylib_cs;

namespace Application;

public enum ParticleType
{
    Air,
    Sand,
    Water,
}

public class Particle
{
    public ParticleType Type { get; set; }
    public bool Updated { get; set; }
    public static readonly Color[] Colors = {Color.BLACK, Color.GOLD, Color.SKYBLUE};

    public Particle()
    {
        Type = ParticleType.Air;
        Updated = false;
    }
}
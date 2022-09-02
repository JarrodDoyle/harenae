using System.Numerics;
namespace Simulation;

public struct ElementSettings
{
    public bool Rise;
    public bool MoveX;
    public bool MoveY;
    public bool MoveXY;
    public string[] Blockers;
    public Vector4 BaseColour;

    public ElementSettings(bool rise, bool moveX, bool moveY, bool moveXY, string[] blockers, Vector4 baseColour)
    {
        Rise = rise;
        MoveX = moveX;
        MoveY = moveY;
        MoveXY = moveXY;
        Blockers = blockers;
        BaseColour = baseColour;
    }
}
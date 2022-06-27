using System.Numerics;
using Raylib_cs;

namespace Application;

public static class SimulationRenderer
{
    private static readonly List<Action> DrawQueue;
    private static readonly Vector2 Dimensions;
    private static readonly RenderTexture2D RenderTexture;

    static SimulationRenderer()
    {
        DrawQueue = new List<Action>();
        Dimensions = new Vector2(320, 180);
        RenderTexture = Raylib.LoadRenderTexture((int)Dimensions.X, (int)Dimensions.Y);
        EnqueueAction(() => Raylib.ClearBackground(Color.BLACK));
    }

    public static void EnqueueAction(Action renderAction)
    {
        DrawQueue.Add(renderAction);
    }

    public static void Render()
    {
        Raylib.BeginTextureMode(RenderTexture);
        foreach (var action in DrawQueue)
            action.Invoke();
        DrawQueue.Clear();
        Raylib.EndTextureMode();
        
        var texture = RenderTexture.texture;
        var source = new Rectangle(0, 0, texture.width, -texture.height);
        var sWidth = Raylib.GetScreenWidth();
        var sHeight = Raylib.GetScreenHeight();
        var scale = (int) Math.Floor(Math.Min(sWidth / Dimensions.X, sHeight / Dimensions.Y));
        var width = (int)(scale * Dimensions.X);
        var height = (int)(scale * Dimensions.Y);
        var dest = new Rectangle((sWidth - width) / 2, (sHeight - height) / 2, width, height);
        Raylib.DrawTexturePro(texture, source, dest, Vector2.Zero, 0, Color.WHITE);
    }
}
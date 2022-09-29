using System.Numerics;
using Raylib_cs;

namespace Application;

public static class SimulationRenderer
{
    private static readonly List<Action> DrawQueue;
    private static readonly Vector2 Dimensions;
    private static readonly RenderTexture2D RenderTexture;
    private static Image Image;

    private static Vector2 DirtyMin;
    private static Vector2 DirtyMax;
    private static bool Dirty;

    static SimulationRenderer()
    {
        DrawQueue = new List<Action>();
        Dimensions = new Vector2(5 * 64, 3 * 64);
        RenderTexture = Raylib.LoadRenderTexture((int)Dimensions.X, (int)Dimensions.Y);
        Image = Raylib.GenImageColor((int)Dimensions.X, (int)Dimensions.Y, Color.BLACK);
        Dirty = true;
        DirtyMin = Vector2.Zero;
        DirtyMax = new Vector2((int)Dimensions.X, (int)Dimensions.Y);
    }

    public static void DrawPixel(int x, int y, Color color)
    {
        if (!Dirty)
        {
            DirtyMin.X = x;
            DirtyMin.Y = y;
            DirtyMax.X = x + 1;
            DirtyMax.Y = y + 1;
        }
        else
        {
            if (x < DirtyMin.X) DirtyMin.X = x;
            if (x >= DirtyMax.X) DirtyMax.X = x + 1;
            if (y < DirtyMin.Y) DirtyMin.Y = y;
            if (y >= DirtyMax.Y) DirtyMax.Y = y + 1;
        }
        Raylib.ImageDrawPixel(ref Image, x, y, color);
        Dirty = true;
    }

    public static void Render()
    {
        var wasDirty = Dirty;
        if (Dirty)
        {
            var dims = DirtyMax - DirtyMin;
            var rect = new Rectangle((int)DirtyMin.X, (int)DirtyMin.Y, (int)dims.X, (int)dims.Y);
            var tmpImage = Raylib.ImageCopy(Image);
            Raylib.ImageCrop(ref tmpImage, rect);
            unsafe
            {
                Raylib.UpdateTextureRec(RenderTexture.texture, rect, tmpImage.data);
            }
            Raylib.UnloadImage(tmpImage);
            Dirty = false;
        }

        var texture = RenderTexture.texture;
        var source = new Rectangle(0, 0, texture.width, texture.height);
        var pos = WorldToScreen(new Vector2(0, 0));
        var destDims = WorldToScreen(Dimensions) - pos;
        var dest = new Rectangle((int)pos.X, (int)pos.Y, (int)destDims.X, (int)destDims.Y);
        Raylib.DrawTexturePro(texture, source, dest, Vector2.Zero, 0, Color.WHITE);

        if (wasDirty) RenderDirtyRect();
    }

    private static void RenderDirtyRect()
    {
        var min = WorldToScreen(DirtyMin);
        var max = WorldToScreen(DirtyMax);

        var dims = max - min;
        var rect = new Rectangle((int)min.X, (int)min.Y, (int)dims.X, (int)dims.Y);
        Raylib.DrawRectangleLinesEx(rect, 1, Color.GREEN);
    }

    public static Vector2 ScreenToWorld(Vector2 pos)
    {
        var sWidth = Raylib.GetScreenWidth();
        var sHeight = Raylib.GetScreenHeight();
        var scale = (int)Math.Floor(Math.Min(sWidth / Dimensions.X, sHeight / Dimensions.Y));
        var offset = new Vector2((sWidth - Dimensions.X * scale) / 2, (sHeight - Dimensions.Y * scale) / 2);
        return (pos - offset) / scale;
    }

    public static Vector2 WorldToScreen(Vector2 pos)
    {
        var sWidth = Raylib.GetScreenWidth();
        var sHeight = Raylib.GetScreenHeight();
        var scale = (int)Math.Floor(Math.Min(sWidth / Dimensions.X, sHeight / Dimensions.Y));
        var offset = new Vector2((sWidth - Dimensions.X * scale) / 2, (sHeight - Dimensions.Y * scale) / 2);
        return (pos * scale) + offset;
    }
}
namespace ColdMint.scripts.map.preview;

using Godot;

public static class RoomPreview
{
    /// <summary>
    /// <para>Create Image</para>
    /// <para>创建图像</para>
    /// </summary>
    /// <param name="groundTileMapLayer"></param>
    /// <returns></returns>
    public static ImageTexture? CreateImage(TileMapLayer? groundTileMapLayer)
    {
        if (groundTileMapLayer == null)
        {
            return null;
        }

        //Calculate the size of the image.
        //计算图像的尺寸。
        //The width is maxX minX and the height is maxY minY.
        //宽度为maxX-minX，高度为maxY-minY。
        var maxY = int.MinValue;
        var maxX = int.MinValue;
        var minY = int.MaxValue;
        var minX = int.MaxValue;
        var cellsArray = groundTileMapLayer.GetUsedCells();
        foreach (var vector2I in cellsArray)
        {
            if (vector2I.Y > maxY)
            {
                maxY = vector2I.Y;
            }

            if (vector2I.Y < minY)
            {
                minY = vector2I.Y;
            }

            if (vector2I.X > maxX)
            {
                maxX = vector2I.X;
            }

            if (vector2I.X < minX)
            {
                minX = vector2I.X;
            }
        }

        var height = maxY - minY;
        var width = maxX - minX;
        var offsetVector2 = Vector2I.Zero - new Vector2I(minX, minY);
        //Create an image.
        //创建image。
        var image = Image.CreateEmpty(width + 1, height + 1, false, Image.Format.Rgba8);
        //image.Fill(Colors.Green);
        //Fill in pixels
        //填充像素点
        foreach (var vector2I in cellsArray)
        {
            image.SetPixel(vector2I.X + offsetVector2.X, vector2I.Y + offsetVector2.Y, new Color(100, 221, 23));
        }

        //Create texture.
        //创建texture
        return ImageTexture.CreateFromImage(image);
    }
}
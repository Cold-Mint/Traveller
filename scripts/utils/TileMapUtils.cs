using Godot;

namespace ColdMint.scripts.utils;

/// <summary>
/// <para>TileMapUtils</para>
/// <para>TileMap工具</para>
/// </summary>
public static class TileMapUtils
{
    /// <summary>
    /// <para>Get the layer number corresponding to LayerName in TileMap</para>
    /// <para>在TileMap内获取LayerName所对应的图层序号</para>
    /// </summary>
    /// <param name="tileMap"></param>
    /// <param name="layerName"></param>
    /// <returns>
    ///<para>− 1 is returned after obtaining failure</para>
    ///<para>获取失败返回-1</para>
    /// </returns>
    public static int GetTileMapLayer(TileMap tileMap, string layerName)
    {
        var count = tileMap.GetLayersCount();
        for (var i = 0; i < count; i++)
        {
            var currentLayerName = tileMap.GetLayerName(i);
            if (currentLayerName == layerName)
            {
                return i;
            }
        }

        return -1;
    }
    
}
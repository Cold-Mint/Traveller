namespace ColdMint.scripts.dataPack;

/// <summary>
/// <para>DataPack</para>
/// <para>数据包</para>
/// </summary>
public interface IDataPack
{
    IDataPackManifest Manifest { get; }

    /// <summary>
    /// <para>Get the item's data</para>
    /// <para>获取物品的数据</para>
    /// </summary>
    /// <returns></returns>
    string GetItemsData();
}
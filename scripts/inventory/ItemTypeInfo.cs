namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>ItemTypeInfo</para>
/// <para>物品类型信息</para>
/// </summary>
/// <param name="Id">
///<para>Id</para>
///<para>物品ID</para>
/// </param>
/// <param name="ScenePath">
///<para>Scene path</para>
///<para>场景路径</para>
/// </param>
/// <param name="IconPath">
///<para>Icon path</para>
///<para>图标路径</para>
/// </param>
/// <param name="MaxStackValue">
///<para>MaxStackValue</para>
///<para>最大堆叠数</para>
/// </param>
/// <param name="TypeCode">
///<para>Item type, value range please see:<see cref="Config.ItemTypeCode"/></para>
///<para>物品类型，取值范围请见：<see cref="Config.ItemTypeCode"/></para>
/// </param>
public record struct ItemTypeInfo(
    string Id,
    string ScenePath,
    string IconPath,
    int MaxStackValue,
    int TypeCode = Config.ItemTypeCode.Unknown);
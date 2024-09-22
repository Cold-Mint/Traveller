namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>IItemDisplay</para>
/// <para>物品显示器</para>
/// </summary>
public interface IItemDisplay
{
    
    /// <summary>
    /// <para>Call this method to refresh the display when the item's information changes</para>
    /// <para>物品的信息发生变更时，调用此方法刷新显示器</para>
    /// </summary>
    /// <remarks>
    ///<param name="item">
    ///<para>New data for items after changes</para>
    ///<para>发生改变后的物品新数据</para>
    /// </param>
    /// </remarks>
    void Update(IItem? item);

    /// <summary>
    /// <para>Show item Display</para>
    /// <para>显示物品显示器</para>
    /// </summary>
    void ShowSelf();
    
    /// <summary>
    /// <para>Hide item Display</para>
    /// <para>隐藏物品显示器</para>
    /// </summary>
    void HideSelf();

}
namespace ColdMint.scripts.loot;

/// <summary>
/// <para>LootDatum</para>
/// <para>战利品数据</para>
/// </summary>
/// <param name="ItemId"></param>
/// <param name="Quantity"></param>
public readonly record struct LootDatum(string ItemId, int Quantity)
{
    public (string id, int quantity) Value
    {
        get => (ItemId, Quantity);
    }
}
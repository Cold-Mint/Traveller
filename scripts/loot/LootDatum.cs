using Godot;

namespace ColdMint.scripts.loot;

public readonly record struct LootDatum(string ItemId, int Quantity)
{
    public (string id, int quantity) Value => (ItemId, Quantity);
}
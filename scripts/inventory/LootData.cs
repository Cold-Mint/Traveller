using Godot;

namespace ColdMint.scripts.inventory;

public partial class LootData : GodotObject
{
    public string? ResPath { get; set; }
    public int Quantity { get; set; }
}
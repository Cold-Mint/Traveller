using ColdMint.scripts.character;
using Godot;

namespace ColdMint.scripts.buff;

public abstract class StatusEffectTemplate : IStatusEffect
{
    public abstract string Id { get; }
    public abstract string Name { get; }
    public abstract Color Color { get; }
    public abstract Config.StatusEffectType StatusEffectType { get; }
    public abstract string Description { get; set; }
    public ulong Duration { get; set; } = 10;
    public int Level { get; set; } = 1;

    public void OnApply(CharacterTemplate character)
    {
    }

    public bool OnTick()
    {
        Duration--;
        return Duration > 0;
    }

    public void OnRemove(CharacterTemplate character)
    {
    }
}
using ColdMint.scripts.barrage;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.pool;

/// <summary>
/// <para>Bullet screen label object pool</para>
/// <para>弹幕标签对象池</para>
/// </summary>
public class BarrageLabelPool : ObjectPool<BarrageLabel>
{
    public PackedScene? BarrageLabelPackedScene { get; set; }

    protected override BarrageLabel? InstantiatePoolable()
    {
        return BarrageLabelPackedScene == null
            ? null
            : NodeUtils.InstantiatePackedScene<BarrageLabel>(BarrageLabelPackedScene);
    }
}
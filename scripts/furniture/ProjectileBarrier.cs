namespace ColdMint.scripts.furniture;

/// <summary>
/// <para>ProjectileBarrier</para>
/// <para>抛射体屏障</para>
/// </summary>
public partial class ProjectileBarrier : Furniture
{
    public override void _Ready()
    {
        base._Ready();
        SetCollisionLayerValue(Config.LayerNumber.ProjectileBarrier, true);
        SetCollisionLayerValue(Config.LayerNumber.Furniture, false);
        SetCollisionMaskValue(Config.LayerNumber.Furniture, true);
        SetCollisionMaskValue(Config.LayerNumber.ProjectileBarrier, true);
        SetCollisionMaskValue(Config.LayerNumber.Barrier, true);
    }
}
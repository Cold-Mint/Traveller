namespace ColdMint.scripts.furniture;

/// <summary>
/// <para>Barrier</para>
/// <para>屏障型家具</para>
/// </summary>
public partial class Barrier : Furniture
{
    public override void _Ready()
    {
        base._Ready();
        CanSleep = false;
        SetCollisionLayerValue(Config.LayerNumber.Barrier, true);
        SetCollisionLayerValue(Config.LayerNumber.Furniture, false);
        SetCollisionMaskValue(Config.LayerNumber.Furniture, true);
        SetCollisionMaskValue(Config.LayerNumber.Barrier, true);
    }
}
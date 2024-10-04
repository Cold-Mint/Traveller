namespace ColdMint.scripts.pickable;

/// <summary>
/// <para>法术</para>
/// </summary>
/// <remarks>
///<para>用于抛射体武器</para>
/// </remarks>
public partial class MagicPickAble : PickAbleTemplate
{
    public override int ItemType
    {
        get => Config.ItemType.Magic;
    }
}
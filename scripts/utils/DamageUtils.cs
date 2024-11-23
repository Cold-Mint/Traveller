using ColdMint.scripts.camp;
using ColdMint.scripts.character;
using ColdMint.scripts.furniture;
using ColdMint.scripts.pickable;
using Godot;

namespace ColdMint.scripts.utils;

public static class DamageUtils
{
    /// <summary>
    /// <para>Detect whether harm is allowed</para>
    /// <para>检测是否允许造成伤害</para>
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool CanCauseHarm(Node2D? owner, Node2D target)
    {
        if (owner == null)
        {
            return false;
        }
        if (owner == target)
        {
            return false;
        }
        if (owner is not CharacterTemplate ownerCharacterTemplate)
        {
            return false;
        }

        if (target is TileMapLayer)
        {
            //When we hit the tile, we return true to prevent the bullet from penetrating the tile.
            //撞击到瓦片时，我们返回true，是为了防止子弹穿透瓦片。
            return true;
        }
        if (target is Furniture)
        {
            return true;
        }
        if (target is PickAbleTemplate pickAbleTemplate)
        {
            //The picked-up item cannot resist the bullet.
            //被拾起的物品无法抵挡子弹。
            return !pickAbleTemplate.Picked;
        }
        if (target is not CharacterTemplate characterTemplate)
        {
            return false;
        }
        //First get the owner's camp and compare it with the target camp
        //先获取主人的阵营与目标阵营进行比较
        var canCauseHarm = CampManager.CanCauseHarm(CampManager.GetCamp(ownerCharacterTemplate.CampId),
            CampManager.GetCamp(characterTemplate.CampId));
        return canCauseHarm;
    }
}
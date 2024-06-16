using System.Collections.Generic;

namespace ColdMint.scripts.loot;

public static class LootRegister
{
    /// <summary>
    /// <para>Register loots hardcoded here</para>
    /// <para>在这里硬编码地注册掉落表</para>
    /// </summary>
    public static void StaticRegister()
    {
        //注册测试使用的战利品表
        if (Config.IsDebug())
        {
            IList<LootGroup> lootGroups = [];
            lootGroups.Add(new LootGroup(0.8,
            [
                new LootEntry("degraded_staff_of_the_undead", weight: 2), new LootEntry("staff_of_the_undead")
            ]));
            lootGroups.Add(new LootGroup(1,
            [
                new LootEntry("packsack", minQuantity: 2, maxQuantity: 4)
            ]));

            var testLootList = new LootList(Config.LootListId.Test, lootGroups);
            LootListManager.RegisterLootList(testLootList);
        }
    }
}
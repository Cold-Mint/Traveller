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
            lootGroups.Add(new LootGroup(1,
            [
                new LootEntry("packsack", weight: 2), new LootEntry("staff_of_the_undead", minQuantity: 2, maxQuantity: 4)
            ]));
            lootGroups.Add(new LootGroup(0.3,
            [
                new LootEntry("packsack")
            ]));

            var testLootList = new LootList(Config.LootListId.Test, lootGroups);
            LootListManager.RegisterLootList(testLootList);
        }
    }
}
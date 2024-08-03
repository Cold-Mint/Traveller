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
        //Register the test using the loot table
        //注册测试使用的战利品表
        if (Config.IsDebug())
        {
            List<LootGroup> lootGroups =
            [
                new LootGroup(0.8f,
                [
                    new LootEntry("staff_necromancy"),
                ])
            ];
            var testLootList = new LootList(Config.LootListId.Test, lootGroups);
            LootListManager.RegisterLootList(testLootList);
        }
    }
}
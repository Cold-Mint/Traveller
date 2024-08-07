﻿namespace ColdMint.scripts.mod;

/// <summary>
/// <para>Mod life cycle handler</para>
/// <para>模组生命周期处理器</para>
/// </summary>
public interface IModLifecycleHandler
{
    /// <summary>
    /// <para>When the game loads the Mod</para>
    /// <para>当游戏加载Mod时</para>
    /// </summary>
    void OnModLoaded();
}
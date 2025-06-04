using ColdMint.scripts.character;

namespace ColdMint.scripts.buff;

public interface IStatusEffect
{
    string Id { get; }

    /// <summary>
    /// <para>The name of buff</para>
    /// <para>buff的名称</para>
    /// </summary>
    string Name { get; }

    /// <summary>
    /// <para>State effect type</para>
    /// <para>状态效果类型</para>
    /// </summary>
    Config.StatusEffectType StatusEffectType { get; }

    /// <summary>
    /// <para>The description of buff</para>
    /// <para>buff的描述</para>
    /// </summary>
    string Description { get; }

    /// <summary>
    /// <para>Remaining duration (seconds)</para>
    /// <para>剩余时长（秒）</para>
    /// </summary>
    long Duration { get; }

    /// <summary>
    /// <para>Level</para>
    /// <para>等级</para>
    /// </summary>
    int Level { get; }

    /// <summary>
    /// <para>When the buff is applied to the character</para>
    /// <para>当buff应用到角色时</para>
    /// </summary>
    void OnApply(CharacterTemplate character);

    /// <summary>
    /// <para>Periodic processing</para>
    /// <para>周期处理</para>
    /// </summary>
    /// <returns>
    ///<para>Return whether it is alive. true means alive, and false will call the OnRemove method.</para>
    ///<para>返回是否存活着，true为活着，false将调用OnRemove方法。</para>
    /// </returns>
    bool OnTick();

    /// <summary>
    /// <para>When the buff is removed from the character(Including automatic removal due to timeout)</para>
    /// <para>当buff从角色中移除时（包括因为超时自动移除）</para>
    /// </summary>
    void OnRemove(CharacterTemplate character);
}
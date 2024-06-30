namespace ColdMint.scripts.stateMachine;

/// <summary>
/// <para>State</para>
/// <para>状态</para>
/// </summary>
public enum State
{
    /// <summary>
    /// <para>idle</para>
    /// <para>空闲</para>
    /// </summary>
    Idle,

    /// <summary>
    /// <para>Look for weapons</para>
    /// <para>寻找武器</para>
    /// </summary>
    LookForWeapon,

    /// <summary>
    /// <para>Chase</para>
    /// <para>追击</para>
    /// </summary>
    /// <remarks>
    ///<para>When the AI character detects the player or target, it enters this state and attempts to chase the target. It can be subdivided into searching target, shortening distance, keeping distance and other sub-states.</para>
    ///<para>当 AI 角色检测到玩家或目标时,进入此状态并尝试追赶目标。可以细分为搜索目标、缩短距离、保持距离等子状态。</para>
    /// </remarks>
    Chase,

    /// <summary>
    /// <para>Attack</para>
    /// <para>攻击</para>
    /// </summary>
    /// <remarks>
    ///<para>This state is entered when the AI character is close to the target and ready to attack. It can be subdivided into the sub-states of selecting attack mode, accumulating power, executing attack, etc.</para>
    ///<para>当 AI 角色接近目标并准备进行攻击时,进入此状态。可以细分为选择攻击方式、蓄力、执行攻击等子状态。</para>
    /// </remarks>
    Attack,


    /// <summary>
    /// <para>Flee</para>
    /// <para>逃跑</para>
    /// </summary>
    Flee,

    /// <summary>
    /// <para>Patrol</para>
    /// <para>巡逻</para>
    /// </summary>
    /// <remarks>
    ///<para>When the AI character is in a goal-less state, it enters this state and patrols the designated area. It can be subdivided into sub-states such as selecting patrol path, moving patrol execution, and detecting abnormal conditions.</para>
    ///<para>当 AI 角色处于无目标状态时,进入此状态并在指定区域内进行巡逻。可以细分为选择巡逻路径、移动执行巡逻、检测异常情况等子状态。</para>
    /// </remarks>
    Patrol,

    /// <summary>
    /// <para>Alert</para>
    /// <para>警戒</para>
    /// </summary>
    /// <remarks>
    ///<para>When the AI character detects something suspicious, it enters this state and alerts. It can be subdivided into sub-states such as scanning the environment, moving to the appropriate position, and remaining alert</para>
    ///<para>当 AI 角色检测到可疑情况时,进入此状态并进行警戒。可以细分为扫描环境、移动至合适位置、保持警戒状态等子状态</para>
    /// </remarks>
    Alert,

    /// <summary>
    /// <para>Interact</para>
    /// <para>互动</para>
    /// </summary>
    /// <remarks>
    ///<para>This state is entered when the AI character needs to interact with the environment or other characters. It can be subdivided into sub-states such as judging the interaction object, choosing the interaction mode, and executing the interaction.</para>
    ///<para>当 AI 角色需要与环境或其他角色进行互动时,进入此状态。可以细分为判断互动对象、选择互动方式、执行互动等子状态。</para>
    /// </remarks>
    Interact
}
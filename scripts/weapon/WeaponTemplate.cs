using System;
using ColdMint.scripts.character;
using ColdMint.scripts.debug;
using ColdMint.scripts.pickable;
using Godot;

namespace ColdMint.scripts.weapon;

/// <summary>
/// <para>WeaponTemplate</para>
/// <para>武器模板</para>
/// </summary>
public abstract partial class WeaponTemplate : PickAbleTemplate
{
    private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    /// <summary>
    /// <para>Fire audio playback component</para>
    /// <para>开火音效播放组件</para>
    /// </summary>
    private AudioStreamPlayer2D? _fireSoundPlayer2D;

    public override void LoadResource()
    {
        base.LoadResource();
        if (_fireSoundPlayer2D == null)
        {
            _fireSoundPlayer2D = GetNode<AudioStreamPlayer2D>("Marker2D/FireSoundPlayer2D");
        }
    }

    public override bool Use(Node2D? owner, Vector2 targetGlobalPosition)
    {
        return Fire(owner, targetGlobalPosition);
    }


    private DateTime? _lastFiringTime;

    /// <summary>
    /// <para>Firing interval</para>
    /// <para>开火间隔</para>
    /// </summary>
    private TimeSpan _firingInterval;

    private long _firingIntervalAsMillisecond = 100;

    [Export]
    protected long FiringIntervalAsMillisecond
    {
        get => _firingIntervalAsMillisecond;
        set
        {
            _firingIntervalAsMillisecond = value;
            _firingInterval = TimeSpan.FromMilliseconds(_firingIntervalAsMillisecond);
        }
    }


    /// <summary>
    /// <para>The recoil of the weapon</para>
    /// <para>武器的后坐力</para>
    /// </summary>
    /// <remarks>
    ///<para>When the weapon is fired, how much recoil is applied to the user, in units: the number of cells, and the X direction of the force is automatically inferred.</para>
    ///<para>武器开火，要对使用者施加多大的后坐力，单位：格数，力的X方向是自动推断的。</para>
    /// </remarks>
    [Export] private long _recoilStrength;

    /// <summary>
    /// <para>Discharge of the weapon</para>
    /// <para>武器开火</para>
    /// </summary>
    /// <remarks>
    ///<param name="owner">
    ///<para>owner</para>
    ///<para>武器所有者</para>
    /// </param>
    /// <param name="enemyGlobalPosition">
    ///<para>enemyGlobalPosition</para>
    ///<para>敌人所在位置</para>
    /// </param>
    /// </remarks>
    public bool Fire(Node2D? owner, Vector2 enemyGlobalPosition)
    {
        var nowTime = DateTime.UtcNow;
        //If the present time minus the time of the last fire is less than the interval between fires, it means that the fire cannot be fired yet.
        //如果现在时间减去上次开火时间小于开火间隔，说明还不能开火。
        if (_lastFiringTime != null && nowTime - _lastFiringTime < _firingInterval)
        {
            return false;
        }
        _lastFiringTime = nowTime;
        var result = DoFire(owner, enemyGlobalPosition);
        if (result)
        {
            if (owner is CharacterTemplate characterTemplate && _recoilStrength != 0)
            {
                characterTemplate.AddForce(enemyGlobalPosition.DirectionTo(characterTemplate.GlobalPosition) * _recoilStrength * Config.CellSize);
            }
            if (_fireSoundPlayer2D == null)
            {
                //No audio player
                //没有音频播放器
                LogCat.Log("no_audio_stream_player");
                return true;
            }

            if (_fireSoundPlayer2D.IsPlaying())
            {
                //The audio is playing
                //音频正在播放中
                LogCat.Log("audio_stream_player_is_playing");
                return true;
            }

            //Play audio
            //播放音频
            LogCat.LogWithFormat("play_audio", LogCat.LogLabel.Default, _fireSoundPlayer2D.Bus);
            _fireSoundPlayer2D.Play();
        }
        return result;
    }

    /// <summary>
    /// <para>Execute fire</para>
    /// <para>执行开火</para>
    /// </summary>
    /// <returns>
    ///<para>Return Is the fire successful?</para>
    ///<para>返回是否成功开火？</para>
    /// </returns>
    protected abstract bool DoFire(Node2D? owner, Vector2 enemyGlobalPosition);
}
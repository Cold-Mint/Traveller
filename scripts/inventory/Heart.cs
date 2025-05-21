using ColdMint.scripts.character;
using ColdMint.scripts.debug;
using ColdMint.scripts.heal;
using ColdMint.scripts.pickable;
using Godot;

namespace ColdMint.scripts.inventory;

public partial class Heart : NonPickupItem
{
    [Export] private AudioStreamPlayer2D? _audioStreamPlayer2D;

    /// <summary>
    /// <para>heal Amount</para>
    /// <para>恢复量</para>
    /// </summary>
    [Export] private int _healAmount; // skipcq:CS-R1137

    [Export] private AudioStream? _playerHealSound;


    public override void _Ready()
    {
        base._Ready();
        if (_audioStreamPlayer2D != null)
        {
            _audioStreamPlayer2D.Stream = _playerHealSound;
        }
    }

    protected override void OnTouchPlayer(Player player)
    {
        base.OnTouchPlayer(player);
        if (!Visible)
        {
            return;
        }

        Hide();
        var heal = new Heal
        {
            HealAmount = _healAmount,
            Source = this,
            MoveLeft = player.FacingLeft
        };
        //When the player touches the heart, the heart must be destroyed, regardless of whether the health is successfully restored.
        //无论是否成功恢复了健康值，在玩家触碰心时，都要销毁心。
        player.Heal(heal);
        if (_audioStreamPlayer2D != null)
        {
            _audioStreamPlayer2D.Finished += AudioStreamPlayer2DOnFinished;
            _audioStreamPlayer2D.Play();
            return;
        }

        FreeSelf();
    }

    private void FreeSelf()
    {
        QueueFree();
    }

    private void AudioStreamPlayer2DOnFinished()
    {
        FreeSelf();
    }
}
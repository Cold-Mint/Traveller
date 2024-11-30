using System.Threading.Tasks;
using ColdMint.scripts.damage;
using Godot;

namespace ColdMint.scripts.character;

/// <summary>
/// <para>PunchBag</para>
/// <para>用于测试武器的沙袋</para>
/// </summary>
public partial class PunchBag : CharacterTemplate
{
    private Vector2 _spawnPoint;
    private bool _initSpawnPoint;

    protected override void OnHit(IDamage damage)
    {
        base.OnHit(damage);
        if (!_initSpawnPoint)
        {
            _spawnPoint = GlobalPosition;
            _initSpawnPoint = true;
        }
    }

    protected override Task OnDie(IDamage damage)
    {
        FullHpRevive();
        GlobalPosition = _spawnPoint;
        return Task.CompletedTask;
    }
}
using ColdMint.scripts.character;
using ColdMint.scripts.debug;
using Godot;

namespace ColdMint.scripts.stateMachine.StateProcessor;

/// <summary>
/// <para>PatrolStateProcessor</para>
/// <para>巡逻状态处理器</para>
/// </summary>
public class PatrolStateProcessor : StateProcessorTemplate
{
    public Vector2[]? Points { get; set; }

    private int _index;
    private Vector2? _originPosition;

    protected override void OnExecute(StateContext context, Node owner)
    {
        if (owner is not AiCharacter aiCharacter)
        {
            return;
        }

        if (Points == null || Points.Length == 0)
        {
            LogCat.LogError("no_points", label: LogCat.LogLabel.PatrolStateProcessor);
            return;
        }

        if (_originPosition == null)
        {
            if (!aiCharacter.IsOnFloor())
            {
                LogCat.LogWarning("patrol_not_on_floor", LogCat.LogLabel.PatrolStateProcessor);
                return;
            }

            _originPosition = aiCharacter.GlobalPosition;
            LogCat.LogWithFormat("patrol_origin_position", LogCat.LogLabel.PatrolStateProcessor,
                _originPosition);
        }

        var point = _originPosition + Points[_index];
        var distance = aiCharacter.GlobalPosition.DistanceTo(point.Value);
        if (distance < 10)
        {
            LogCat.LogWithFormat("patrol_arrival_point", LogCat.LogLabel.PatrolStateProcessor, point);
            _index++;
            if (_index >= Points.Length)
            {
                _index = 0;
            }
        }
        else
        {
            LogCat.LogWithFormat("patrol_to_next_point", label: LogCat.LogLabel.PatrolStateProcessor, point,
                aiCharacter.GlobalPosition, Points[_index],
                distance);
            aiCharacter.SetTargetPosition(point.Value);
        }
    }


    public override State State => State.Patrol;
}
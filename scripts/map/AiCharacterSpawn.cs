using ColdMint.scripts.character;
using ColdMint.scripts.map.room;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.map;

/// <summary>
/// <para>Ai character generation point</para>
/// <para>Ai角色生成点</para>
/// </summary>
public partial class AiCharacterSpawn : Marker2D, ISpawnMarker
{
    private PackedScene? _packedScene;
    [Export] private string? _resPath;

    public override void _Ready()
    {
        base._Ready();
        if (!string.IsNullOrEmpty(_resPath))
        {
            _packedScene = GD.Load<PackedScene>(_resPath);
        }
    }

    public Node2D? Spawn()
    {
        if (GameSceneDepend.AiCharacterContainer == null || _packedScene == null)
        {
            return null;
        }

        var aiCharacter = NodeUtils.InstantiatePackedScene<AiCharacter>(_packedScene);
        if (aiCharacter == null)
        {
            return null;
        }

        NodeUtils.CallDeferredAddChild(GameSceneDepend.AiCharacterContainer, aiCharacter);
        aiCharacter.GlobalPosition = GlobalPosition;
        return aiCharacter;
    }

    public bool CanQueueFree()
    {
        return true;
    }

    public void DoQueueFree()
    {
        QueueFree();
    }
}
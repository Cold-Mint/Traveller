using ColdMint.scripts.character;
using ColdMint.scripts.map.events;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.map;

/// <summary>
/// <para>Ai character generation point</para>
/// <para>Ai角色生成点</para>
/// </summary>
public partial class AiCharacterSpawn : Marker2D
{
    private PackedScene? _packedScene;

    public override void _Ready()
    {
        base._Ready();
        var resPath = GetMeta("ResPath", Name).AsString();
        if (!string.IsNullOrEmpty(resPath))
        {
            _packedScene = GD.Load<PackedScene>(resPath);
        }

        EventManager.AiCharacterGenerateEvent += OnAiCharacterGenerateEvent;
    }

    /// <summary>
    /// <para>When an event is triggered</para>
    /// <para>当触发事件时</para>
    /// </summary>
    /// <param name="aiCharacterGenerateEvent"></param>
    public void OnAiCharacterGenerateEvent(AiCharacterGenerateEvent aiCharacterGenerateEvent)
    {
        if (GameSceneNodeHolder.AiCharacterContainer == null)
        {
            return;
        }

        if (_packedScene == null)
        {
            return;
        }

        var aiCharacter = NodeUtils.InstantiatePackedScene<AiCharacter>(_packedScene);
        if (aiCharacter == null)
        {
            return;
        }

        NodeUtils.CallDeferredAddChild(GameSceneNodeHolder.AiCharacterContainer, aiCharacter);
        aiCharacter.GlobalPosition = GlobalPosition;
    }

    public override void _ExitTree()
    {
        EventManager.AiCharacterGenerateEvent -= OnAiCharacterGenerateEvent;
    }
}
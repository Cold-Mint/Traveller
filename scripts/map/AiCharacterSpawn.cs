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
    [Export] private string[]? _resPathArray;

    public Node2D? Spawn(int waveNumber)
    {
        if (GameSceneDepend.AiCharacterContainer == null)
        {
            return null;
        }
        if (_resPathArray == null)
        {
            return null;
        }
        if (waveNumber < 0 || waveNumber >= _resPathArray.Length)
        {
            return null;
        }
        var resPath = _resPathArray[waveNumber];
        if (string.IsNullOrEmpty(resPath))
        {
            return null;
        }
        var packedScene = GD.Load<PackedScene>(resPath);
        if (packedScene == null)
        {
            return null;
        }
        var aiCharacter = NodeUtils.InstantiatePackedScene<AiCharacter>(packedScene);
        if (aiCharacter == null)
        {
            return null;
        }

        NodeUtils.CallDeferredAddChild(GameSceneDepend.AiCharacterContainer, aiCharacter);
        aiCharacter.GlobalPosition = GlobalPosition;
        return aiCharacter;
    }

    public int GetMaxWaveNumber()
    {
        return _resPathArray?.Length ?? 0;
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
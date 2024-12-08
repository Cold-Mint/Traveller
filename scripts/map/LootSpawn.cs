using System.Threading.Tasks;
using ColdMint.scripts.loot;
using ColdMint.scripts.map.room;
using Godot;

namespace ColdMint.scripts.map;

public partial class LootSpawn : Marker2D, ISpawnMarker
{

    [Export] private string[]? _lootIdList;


    public async Task<Node2D[]?> Spawn(int waveNumber)
    {
        if (_lootIdList == null || _lootIdList.IsEmpty())
        {
            return null;
        }
        if (waveNumber < 0 || waveNumber >= _lootIdList.Length)
        {
            return null;
        }
        var lootId = _lootIdList[waveNumber];
        if (string.IsNullOrEmpty(lootId))
        {
            return null;
        }
        return await LootListManager.GenerateLootObjectsAsync<Node2D>(GetParent(), LootListManager.GenerateLootData(lootId), GlobalPosition);
    }

    public int GetMaxWaveNumber()
    {
        return _lootIdList?.Length ?? 0;
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
using System.Threading.Tasks;
using ColdMint.scripts.map.interfaces;
using ColdMint.scripts.serialization;
using Godot;

namespace ColdMint.scripts.map.roomInjectionProcessor;

/// <summary>
/// <para>Room injection processor template</para>
/// <para>房间注入处理器模板</para>
/// </summary>
/// <typeparam name="TConfig"></typeparam>
public abstract class RoomInjectionProcessorTemplate<TConfig> : IRoomInjectionProcessor
{
    public abstract string GetId();

    public Task<bool> CanBePlaced(RandomNumberGenerator randomNumberGenerator, string? yamlConfigData)
    {
        if (yamlConfigData == null)
        {
            return Task.FromResult(false);
        }

        var configData = YamlSerialization.Deserialize<TConfig>(yamlConfigData);
        return configData == null ? Task.FromResult(false) : OnCreateConfigData(randomNumberGenerator, configData);
    }

    /// <summary>
    /// <para>When creating a configuration class</para>
    /// <para>当创建配置类时</para>
    /// </summary>
    /// <param name="randomNumberGenerator"></param>
    /// <param name="configData"></param>
    /// <returns></returns>
    protected abstract Task<bool> OnCreateConfigData(RandomNumberGenerator randomNumberGenerator, TConfig configData);
}
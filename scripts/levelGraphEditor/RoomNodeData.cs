using System.Linq;

namespace ColdMint.scripts.levelGraphEditor;

public class RoomNodeData
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }

    public string[]? RoomTemplateSet { get; set; }

    public string[]? Tags { get; set; }

    /// <summary>
    /// <para>Room injector data</para>
    /// <para>房间注入器数据</para>
    /// </summary>
    public string? RoomInjectionProcessorData { get; set; }

    // ReSharper disable UnusedAutoPropertyAccessor.Global
    public string? EnterRoomEventHandlerId { get; set; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global

    // ReSharper disable UnusedAutoPropertyAccessor.Global
    public string? ExitRoomEventHandlerId { get; set; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global

    /// <summary>
    /// <para>Whether a tag is held</para>
    /// <para>是否持有某个标签</para>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool HasTag(string tag)
    {
        return Tags != null && Tags.Any(t => t == tag);
    }
}
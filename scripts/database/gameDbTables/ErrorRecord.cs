using System;
using System.ComponentModel.DataAnnotations;

namespace ColdMint.scripts.database.gameDbTables;

/// <summary>
/// <para>Error record</para>
/// <para>错误记录</para>
/// </summary>
public class ErrorRecord
{
    /// <summary>
    /// <para>Message</para>
    /// <para>错误消息</para>
    /// </summary>
    [Key]
    [MaxLength(255)]
    public string? Message { get; set; }

    /// <summary>
    /// <para>Count</para>
    /// <para>出现次数</para>
    /// </summary>
    public int Count { get; set; } = 1;

    /// <summary>
    /// <para>DateTime</para>
    /// <para>时间</para>
    /// </summary>
    public DateTime LastDateTime { get; set; } = DateTime.Now;
}
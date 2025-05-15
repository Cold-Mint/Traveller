using System.Collections.Generic;
using Godot;

namespace ColdMint.scripts.console.objectSelector;

/// <summary>
/// <para>The data source of the object selector</para>
/// <para>对象选择器的数据源</para>
/// </summary>
public interface IObjectSelectorDataSource
{
    /// <summary>
    /// <para>The object types contained within the data source</para>
    /// <para>数据源内包含的对象类型</para>
    /// </summary>
    int ObjectType { get; }

    /// <summary>
    /// <para>Query</para>
    /// <para>查询数据源</para>
    /// </summary>
    /// <returns></returns>
    public void Query(ObjectSelectorQueryRequest request, ref List<Node> resultList);
}
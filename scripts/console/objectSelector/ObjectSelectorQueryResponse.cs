using System;
using System.Collections.Generic;
using Godot;

namespace ColdMint.scripts.console.objectSelector;

public class ObjectSelectorQueryResponse
{
    private List<Node> _resultList;

    public int Count;

    public ObjectSelectorQueryResponse(List<Node> resultList)
    {
        _resultList = resultList;
        Count = _resultList.Count;
    }


    /// <summary>
    /// <para>Filter the objects of the specified type within the result of the response</para>
    /// <para>在响应的结果内过滤指定类型的对象</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public void Filter<T>(Action<T> action) where T : Node
    {
        _resultList.ForEach(node =>
        {
            if (node is T t)
            {
                action.Invoke(t);
            }
        });
    }
}
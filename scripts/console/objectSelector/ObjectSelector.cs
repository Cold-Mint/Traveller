using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ColdMint.scripts.console.objectSelector;

/// <summary>
/// <para>Be responsible for finding the objects in the game</para>
/// <para>负责查找游戏内的对象</para>
/// </summary>
public class ObjectSelector
{
    private static readonly List<IObjectSelectorDataSource> DataSources = [];

    /// <summary>
    /// <para>Register a data source</para>
    /// <para>注册一个数据源</para>
    /// </summary>
    /// <param name="dataSource"></param>
    public static void Register(IObjectSelectorDataSource dataSource)
    {
        DataSources.Add(dataSource);
    }

    public static ObjectSelectorQueryResponse Query(ObjectSelectorQueryRequest request)
    {
        var resultList = new List<Node>();
        foreach (var objectSelectorDataSource in from objectSelectorDataSource in DataSources
                 let objectType = objectSelectorDataSource.ObjectType
                 where request.Type == Config.ObjectType.All || objectType == request.Type
                 select objectSelectorDataSource)
        {
            objectSelectorDataSource.Query(request, ref resultList);
        }

        return new ObjectSelectorQueryResponse(resultList);
    }
}
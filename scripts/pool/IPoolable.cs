namespace ColdMint.scripts.pool;

/// <summary>
/// <para>An element that can be placed in an object pool</para>
/// <para>可放入对象池内的元素</para>
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// <para>Whether it can be recycled</para>
    /// <para>是否可以回收</para>
    /// </summary>
    /// <returns>
    ///<para>If true is returned, the object is placed at the end of the object pool and reused</para>
    ///<para>如果返回true，则会将此对象放入对象池的末尾，并且复用该对象</para>
    /// </returns>
    bool CanRecycle();
}
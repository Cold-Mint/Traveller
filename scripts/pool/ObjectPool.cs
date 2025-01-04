using System.Collections.Generic;

namespace ColdMint.scripts.pool;

/// <summary>
/// <para>An object pool of dynamic capacity</para>
/// <para>动态容量的对象池</para>
/// </summary>
public abstract class ObjectPool<T> where T : IPoolable
{
    private readonly Queue<T?> _poolQueue = new();

    /// <summary>
    /// <para>Instantiate reusable objects</para>
    /// <para>实例化可复用对象</para>
    /// </summary>
    /// <returns></returns>
    protected abstract T? InstantiatePoolable();

    /// <summary>
    /// <para>Gets a reusable object from the object pool</para>
    /// <para>从对象池中获取一个可复用对象</para>
    /// </summary>
    /// <remarks>
    /// <para>If there is a reusable object available in the object pool, reuse it, otherwise create a new instance.</para>
    /// <para>如果对象池中有可用的可复用对象，则复用它，否则创建一个新的实例。</para>
    /// </remarks>
    /// <returns></returns>
    public T? AcquirePoolable(out bool isNew)
    {
        if (_poolQueue.TryPeek(out var poolableAtFront) && poolableAtFront?.CanRecycle() == true &&
            _poolQueue.TryDequeue(out var poolableToUse))
        {
            //Retrieves the reusable object from the beginning of the queue
            //从队列开头取出可复用对象
            _poolQueue.Enqueue(poolableToUse);
            isNew = false;
            poolableToUse?.OnRecycle();
            return poolableToUse;
        }

        var poolable = InstantiatePoolable();
        _poolQueue.Enqueue(poolable);
        isNew = true;
        return poolable;
    }
}
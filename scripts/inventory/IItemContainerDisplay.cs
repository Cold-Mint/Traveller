using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColdMint.scripts.inventory;

public interface IItemContainerDisplay : IEnumerable<IItemDisplay>
{
    /// <summary>
    /// <para>Bind an item container to the item container display</para>
    /// <para>为物品容器显示器绑定物品容器</para>
    /// </summary>
    /// <param name="itemContainer"></param>
    Task BindItemContainer(IItemContainer? itemContainer);
}
using System.Threading.Tasks;

namespace ColdMint.scripts.loader.sceneLoader;

public interface ISceneLoaderContract
{
    /// <summary>
    /// <para>initialization data</para>
    /// <para>初始化数据</para>
    /// </summary>
    Task InitializeData();

    /// <summary>
    /// <para>load scene</para>
    /// <para>加载场景</para>
    /// </summary>
    Task LoadScene();
}
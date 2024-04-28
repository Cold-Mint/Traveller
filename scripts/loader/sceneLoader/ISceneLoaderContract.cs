namespace ColdMint.scripts.loader.sceneLoader;

public interface ISceneLoaderContract
{
    /// <summary>
    /// <para>initialization data</para>
    /// <para>初始化数据</para>
    /// </summary>
    void InitializeData();

    /// <summary>
    /// <para>load scene</para>
    /// <para>加载场景</para>
    /// </summary>
    void LoadScene();
}
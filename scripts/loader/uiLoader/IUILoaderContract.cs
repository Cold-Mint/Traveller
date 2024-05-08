namespace ColdMint.scripts.loader.uiLoader;

/// <summary>
/// <para>UI Loader Contract</para>
/// <para>UI加载器契约</para>
/// </summary>
/// <remarks>
///<para>Define methods that must be implemented by all UI loaders within the project</para>
///<para>为项目内所有UI加载器制定一些必须实现的方法</para>
/// </remarks>
public interface IUiLoaderContract
{
    /// <summary>
    /// <para>This method loads the information that the UI needs to display, such as: button text, images, etc.</para>
    /// <para>在此方法内加载UI需要显示的信息，例如：按钮的文字，图像等。</para>
    /// </summary>
    void InitializeUi();

    /// <summary>
    /// <para>initialization data</para>
    /// <para>初始化数据</para>
    /// </summary>
    void InitializeData();

    /// <summary>
    /// <para>Load user actions that the UI needs to respond to, such as setting a click event for a button.</para>
    /// <para>加载UI需要响应的用户行动，例如为按钮设置点击事件。</para>
    /// </summary>
    void LoadUiActions();
}
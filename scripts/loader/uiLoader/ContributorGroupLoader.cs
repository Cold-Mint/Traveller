using ColdMint.scripts.contribute;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.loader.uiLoader;

/// <summary>
/// <para>Contributor group loader</para>
/// <para>贡献者组加载器</para>
/// </summary>
public partial class ContributorGroupLoader : UiLoaderTemplate
{
    private string? _title;
    private ContributorData[]? _contributorDataArray;

    /// <summary>
    /// <para>Contributor array</para>
    /// <para>贡献者数组</para>
    /// </summary>
    public ContributorData[]? ContributorDataArray
    {
        get => _contributorDataArray;
        set
        {
            _contributorDataArray = value;
            SetContributorData(value);
        }
    }

    /// <summary>
    /// <para>The title of the contribution group</para>
    /// <para>贡献组的标题</para>
    /// </summary>
    public string? Title
    {
        get => _title;
        set
        {
            _title = value;
            SetTitle(value);
        }
    }

    private Label? _titleLabel;
    private HFlowContainer? _flowContainer;

    public override void InitializeUi()
    {
        _titleLabel = GetNode<Label>("TitleLabel");
        _flowContainer = GetNode<HFlowContainer>("HFlowContainer");
        //If there is initial data, it is loaded at startup.
        //如果有初始数据，那么在启动时加载。
        SetTitle(_title);
        SetContributorData(_contributorDataArray);
    }

    /// <summary>
    /// <para>Set contributor array</para>
    /// <para>设置贡献者数组</para>
    /// </summary>
    /// <param name="contributorDataArray"></param>
    private void SetContributorData(ContributorData[]? contributorDataArray)
    {
        if (_flowContainer == null)
        {
            return;
        }
        NodeUtils.DeleteAllChild(_flowContainer);
        if (contributorDataArray == null || contributorDataArray.Length == 0)
        {
            return;
        }
        foreach (var contributorData in contributorDataArray)
        {
            var linkButton = new LinkButton();
            linkButton.Underline = LinkButton.UnderlineMode.OnHover;
            linkButton.Text = contributorData.Name;
            linkButton.Uri = contributorData.Url;
            var toolTip = contributorData.ToolTip;
            if (toolTip == null)
            {
                linkButton.TooltipText = contributorData.Url;
            }
            else
            {
                linkButton.TooltipText = contributorData.ToolTip;
            }
            _flowContainer.AddChild(linkButton);
        }
    }

    /// <summary>
    /// <para>Set Title</para>
    /// <para>设置标题</para>
    /// </summary>
    /// <param name="title"></param>
    private void SetTitle(string? title)
    {
        if (_titleLabel != null)
        {
            if (title == null)
            {
                _titleLabel.Hide();
            }
            else
            {
                _titleLabel.Show();
                _titleLabel.Text = title;
            }
        }
    }
}

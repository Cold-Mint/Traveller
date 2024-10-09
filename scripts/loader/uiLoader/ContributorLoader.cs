using ColdMint.scripts.contribute;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.loader.uiLoader;

/// <summary>
/// <para>ContributorLoader</para>
/// <para>贡献者页面加载器</para>
/// </summary>
public partial class ContributorLoader : UiLoaderTemplate
{
    private Button? _exitButton;
    private VBoxContainer? _boxContainer;
    private PackedScene? _packedScene;
    private PackedScene? _mainMenu;
    private PackedScene? _contributorGroup;

    public override void InitializeData()
    {
        _mainMenu = ResourceLoader.Load<PackedScene>("res://scenes/mainMenu.tscn");
        _packedScene = ResourceLoader.Load<PackedScene>("res://prefab/ui/contributorGroup.tscn");
        _contributorGroup = ResourceLoader.Load<PackedScene>("res://prefab/ui/contributorGroup.tscn");
    }

    public override void InitializeUi()
    {
        _exitButton = GetNode<Button>("ExitButton");
        _boxContainer = GetNode<VBoxContainer>("VBoxContainer");
        var dictionary =
            ContributorDataManager.GetContributorTypeToContributorDataArray();
        if (dictionary != null)
        {
            foreach (var contributorType in dictionary.Keys)
            {
                AddGroup(ContributorDataManager.ContributorTypeToString(contributorType), dictionary[contributorType]);
            }
        }
    }

    private void AddGroup(string? title, ContributorData[] contributorDataArray)
    {
        if (_boxContainer == null || _contributorGroup == null)
        {
            return;
        }

        var contributorGroupLoader = NodeUtils.InstantiatePackedScene<ContributorGroupLoader>(_contributorGroup);
        if (contributorGroupLoader != null)
        {
            contributorGroupLoader.Title = title;
            contributorGroupLoader.ContributorDataArray = contributorDataArray;
            _boxContainer.AddChild(contributorGroupLoader);
        }
    }

    public override void LoadUiActions()
    {
        if (_exitButton != null)
        {
            _exitButton.Pressed += () =>
            {
                if (_mainMenu == null)
                {
                    return;
                }

                GetTree().ChangeSceneToPacked(_mainMenu);
            };
        }
    }
}

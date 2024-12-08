using System;
using System.Threading.Tasks;
using ColdMint.scripts.debug;
using Godot;

namespace ColdMint.scripts.loader.sceneLoader;

/// <summary>
/// <para>The scene loader template</para>
/// <para>场景加载器模板</para>
/// </summary>
public partial class SceneLoaderTemplate : Node2D, ISceneLoaderContract
{
    public sealed override async void _Ready()
    {
        try
        {
            await InitializeData();
            await LoadScene();
        }
        catch (Exception e)
        {
            LogCat.WhenCaughtException(e);
        }
    }


    public virtual Task InitializeData()
    {
        return Task.CompletedTask;
    }

    public virtual Task LoadScene()
    {
        return Task.CompletedTask;
    }
}
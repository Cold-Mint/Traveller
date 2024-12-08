using System.Threading.Tasks;
using Godot;

namespace ColdMint.scripts.loader.sceneLoader;

/// <summary>
/// <para>The scene loader template</para>
/// <para>场景加载器模板</para>
/// </summary>
public partial class SceneLoaderTemplate : Node2D, ISceneLoaderContract
{
    public sealed override void _Ready()
    {
        InitializeData().GetAwaiter().OnCompleted(() => LoadScene());
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
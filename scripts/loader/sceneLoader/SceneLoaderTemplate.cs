using Godot;

namespace ColdMint.scripts.loader.sceneLoader;

/// <summary>
/// <para>场景加载器模板</para>
/// <para>场景加载器模板</para>
/// </summary>
public partial class SceneLoaderTemplate : Node2D, ISceneLoaderContract
{
	public sealed override void _Ready()
	{
		InitializeData();
		LoadScene();
	}

	public virtual void InitializeData()
	{
		
	}

	public virtual void LoadScene()
	{
		
	}
}

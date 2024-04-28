using Godot;

namespace ColdMint.scripts.loader.uiLoader;

public partial class UiLoaderTemplate : Control, IUiLoaderContract
{
	/// <summary>
	/// <para>The sealed keyword prevents subclasses from overriding the method</para>
	/// <para>sealed关键字的作用时禁止子类重写该方法</para>
	/// </summary>
	public sealed override void _Ready()
	{
		InitializeData();
		InitializeUI();
		LoadUIActions();
	}

   
	public virtual void InitializeUI()
	{
	}

	public virtual void InitializeData()
	{
		
	}

	public virtual void LoadUIActions()
	{
	}
}
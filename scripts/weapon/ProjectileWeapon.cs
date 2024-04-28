using System.Collections.Generic;
using ColdMint.scripts.character;
using ColdMint.scripts.debug;
using ColdMint.scripts.projectile;
using Godot;

namespace ColdMint.scripts.weapon;

public partial class ProjectileWeapon : WeaponTemplate
{
	private Marker2D _marker2D;
	private string[] _projectiles;
	private Dictionary<string, PackedScene> _projectileCache;
	private Node2D _projectileContainer;

	public override void _Ready()
	{
		base._Ready();
		_marker2D = GetNode<Marker2D>("Marker2D");
		_projectileCache = new Dictionary<string, PackedScene>();
		_projectiles = GetMeta("Projectiles", "").AsStringArray();
		foreach (var projectileItem in _projectiles)
		{
			var packedScene = GD.Load<PackedScene>(projectileItem);
			_projectileCache.Add(projectileItem, packedScene);
		}

		_projectileContainer = GetNode("/root/Game/ProjectileContainer") as Node2D;
	}


	protected override void DoFire(Node2D owner, Vector2 enemyGlobalPosition)
	{
		if (_projectiles.IsEmpty())
		{
			LogCat.LogError("projectiles_is_empty");
			return;
		}


		//Get the first projectile
		var projectileScene = _projectileCache[_projectiles[0]];
		var projectile = projectileScene.Instantiate() as ProjectileTemplate;
		projectile.Owner = owner;
		var vector2 = (enemyGlobalPosition - _marker2D.GlobalPosition).Normalized() * projectile.Speed;
		projectile.Velocity = vector2;
		projectile.Position = _marker2D.GlobalPosition;
		_projectileContainer.AddChild(projectile);
	}
}

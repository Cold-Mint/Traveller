using System;
using System.Collections.Generic;
using ColdMint.scripts.pool;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.barrage;

/// <summary>
/// <para>BarrageNode</para>
/// <para>弹幕节点</para>
/// </summary>
public partial class BarrageNode : Control
{
	[Export] private PackedScene? _packedScene;
	private readonly BarrageLabelPool _barrageLabelPool = new();
	private readonly List<BarrageData> _barrageDataList = new();
	private int _index;
	private DateTime _nextLaunchTime = DateTime.MinValue;

	public override void _Ready()
	{
		base._Ready();
		_barrageLabelPool.BarrageLabelPackedScene = _packedScene;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		var nowTime = DateTime.UtcNow;
		if (nowTime < _nextLaunchTime)
		{
			return;
		}

		if (_barrageDataList.Count == 0)
		{
			return;
		}

		var barrageData = _barrageDataList[_index];
		var barrageLabel = _barrageLabelPool.AcquirePoolable(out var isNew);
		if (barrageLabel == null)
		{
			return;
		}

		if (isNew)
		{
			NodeUtils.CallDeferredAddChild(this, barrageLabel);
		}

		var position = new Vector2(0, RandomUtils.Instance.Next(0, 50));
		barrageLabel.Position = position;
		barrageLabel.Text = barrageData.Text;
		_nextLaunchTime = nowTime.Add(barrageData.Duration);
		_index = (_index + 1) % _barrageDataList.Count;
	}
}

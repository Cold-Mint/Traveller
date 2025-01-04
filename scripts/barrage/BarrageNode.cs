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
	private readonly List<BarrageData> _barrageDataList = [];
	private int _index;
	private DateTime _nextLaunchTime = DateTime.MinValue;
	private RandomNumberGenerator _randomNumberGenerator = new();

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

		barrageLabel.SetLabelText(barrageData.Text);
		var position = new Vector2(-barrageLabel.GetContentWidth(),
			_randomNumberGenerator.RandfRange(0, GetWindow().Size.Y * 0.6f));
		barrageLabel.Position = position;
		_nextLaunchTime = nowTime.Add(barrageData.Duration);
		_index = (_index + 1) % _barrageDataList.Count;
	}
}

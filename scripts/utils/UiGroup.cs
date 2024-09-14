using System.Collections.Generic;
using Godot;

namespace ColdMint.scripts.utils;

/// <summary>
/// <para>UI Group</para>
/// <para>UI组</para>
/// </summary>
public partial class UiGroup : Control
{
    private readonly HashSet<Control> _visibleControls = [];
    private readonly HashSet<Control> _allControls = [];

    /// <summary>
    /// <para>How many nodes are visible</para>
    /// <para>有多少个节点处于可见状态</para>
    /// </summary>
    public int VisibleControlsCount => _visibleControls.Count;

    /// <summary>
    /// <para>Register nodes in the UI group. For registered nodes, do not use <see cref="Godot.CanvasItem.Show"/> or <see cref="Godot.CanvasItem.Hide"/> to change the visible state. Call the <see cref="ShowControl"/> and <see cref="HideControl"/> methods instead.</para>
    /// <para>注册节点到UI组内，对于已注册的节点，不要直接使用<see cref="Godot.CanvasItem.Show"/>或<see cref="Godot.CanvasItem.Hide"/>方法来改变可见状态，请调用<see cref="ShowControl"/>和<see cref="HideControl"/>方法来替代他们。</para>
    /// </summary>
    /// <param name="control"></param>
    public void RegisterControl(Control control)
    {
        control.TreeExited += () => { OnTreeExited(control); };
        NodeUtils.CallDeferredAddChild(this, control);
        _allControls.Add(control);
    }

    /// <summary>
    /// <para>Hide all nodes</para>
    /// <para>隐藏全部节点</para>
    /// </summary>
    public void HideAllControl()
    {
        if (_visibleControls.Count == 0)
        {
            return;
        }

        foreach (var visibleControl in _visibleControls)
        {
            visibleControl.Hide();
        }

        _visibleControls.Clear();
        Hide();
    }

    /// <summary>
    /// <para>Hide a node</para>
    /// <para>隐藏某个节点</para>
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public bool HideControl(Control control)
    {
        if (!control.IsVisible())
        {
            return false;
        }

        if (!_allControls.Contains(control))
        {
            return false;
        }

        control.Hide();
        _visibleControls.Remove(control);
        ChangeSelfVisibility();
        return true;
    }

    /// <summary>
    /// <para>Show node</para>
    /// <para>显示某个节点</para>
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public bool ShowControl(Control control)
    {
        if (control.IsVisible())
        {
            return false;
        }

        if (!_allControls.Contains(control))
        {
            return false;
        }

        control.Show();
        _visibleControls.Add(control);
        ChangeSelfVisibility();
        return true;
    }

    /// <summary>
    /// <para>ChangeSelfVisibility</para>
    /// <para>改变自身的可见度</para>
    /// </summary>
    private void ChangeSelfVisibility()
    {
        if (_visibleControls.Count == 0)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void OnTreeExited(Control control)
    {
        //The Hide method is not called when a node exits from the tree, so remove the node here to prevent empty references.
        //当节点从节点树内退出时，并不会调用Hide方法，所以在这里移除节点，防止产生空引用。
        _visibleControls.Remove(control);
        _allControls.Remove(control);
    }
}
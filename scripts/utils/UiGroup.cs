using System;
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
    private readonly Dictionary<string, Func<Control?>> _controlFunc = new();

    /// <summary>
    /// <para>Holds the node that has been instantiated</para>
    /// <para>持有已实例化的节点</para>
    /// </summary>
    private readonly Dictionary<string, Control> _instantiatedControl = new();

    /// <summary>
    /// <para>Registered control node</para>
    /// <para>注册控制节点</para>
    /// </summary>
    /// <param name="key">
    ///<para>key</para>
    ///<para>控制节点的key</para>
    /// </param>
    /// <param name="func">
    ///<para>Creates a function to control the node. UiGroup delays calling this function to create the node.</para>
    ///<para>创建控制节点的函数，UiGroup会延迟调用这个函数创建节点。</para>
    /// </param>
    public void RegisterControl(string key, Func<Control?> func)
    {
        _controlFunc.TryAdd(key, func);
    }


    /// <summary>
    /// <para>Obtain or create a controller node</para>
    /// <para>获取或者创建控制节点</para>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private Control? GetOrCreateControl(string key)
    {
        if (_instantiatedControl.TryGetValue(key, out var instantiatedControl))
        {
            return instantiatedControl;
        }

        if (!_controlFunc.TryGetValue(key, out var func))
        {
            return null;
        }

        var control = func.Invoke();
        if (control == null)
        {
            return null;
        }
        control.Hide();
        control.TreeExited += () => { OnTreeExited(key, control); };
        NodeUtils.CallDeferredAddChild(this, control);
        _instantiatedControl.Add(key, control);
        return control;
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
    /// <param name="key"></param>
    /// <returns></returns>
    public bool HideControl(string key)
    {
        if (!_instantiatedControl.TryGetValue(key, out var control))
        {
            return false;
        }

        return HideControl(control);
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

        control.Hide();
        _visibleControls.Remove(control);
        ChangeSelfVisibility();
        return true;
    }

    /// <summary>
    /// <para>Show node</para>
    /// <para>显示某个节点</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="beforeDisplayControl">
    ///<para>A callback function before the display node where you can generate rendered page content. For example, set the title</para>
    ///<para>在显示节点之前的回调函数，您可以在此函数内生成渲染页面内容。例如：设置标题</para>
    /// </param>
    /// <returns></returns>
    public bool ShowControl(string key, Action<Control>? beforeDisplayControl = null)
    {
        var control = GetOrCreateControl(key);
        if (control == null)
        {
            return false;
        }

        if (control.IsVisible())
        {
            return false;
        }

        if (beforeDisplayControl != null)
        {
            beforeDisplayControl.Invoke(control);
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

    private void OnTreeExited(string key, Control control)
    {
        //The Hide method is not called when a node exits from the tree, so remove the node here to prevent empty references.
        //当节点从节点树内退出时，并不会调用Hide方法，所以在这里移除节点，防止产生空引用。
        _visibleControls.Remove(control);
        _instantiatedControl.Remove(key);
    }
}
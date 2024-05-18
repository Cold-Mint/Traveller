using Godot;

namespace ColdMint.scripts.nodeBinding;

/// <summary>
/// <para>Node binding</para>
/// <para>节点绑定</para>
/// </summary>
public interface INodeBinding
{
    void Binding(Node root);
}
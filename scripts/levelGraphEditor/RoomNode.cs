using Godot;

namespace ColdMint.scripts.levelGraphEditor;

public partial class RoomNode : GraphNode
{
    private Label? _describeLabel;
    private IRoomNodeData? _roomNodeData;

    public IRoomNodeData? RoomNodeData
    {
        get => _roomNodeData;
        set
        {
            if (_describeLabel == null || value == null) return;
            Title = value.Title;
            if (value.Description.Length == 0)
            {
                _describeLabel.Visible = false;
            }
            else
            {
                _describeLabel.Visible = true;
                _describeLabel.Text = value.Description;
            }

            _roomNodeData = value;
        }
    }

    public override void _Ready()
    {
        base._Ready();
        _describeLabel = GetNode<Label>("DescribeLabel");
    }
    
}
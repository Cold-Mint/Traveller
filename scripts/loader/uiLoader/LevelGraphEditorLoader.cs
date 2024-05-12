using ColdMint.scripts.levelGraphEditor;
using Godot;

namespace ColdMint.scripts.loader.uiLoader;

/// <summary>
/// <para>Level graph editor</para>
/// <para>关卡图编辑器</para>
/// </summary>
public partial class LevelGraphEditorLoader : UiLoaderTemplate
{
    private GraphEdit? _graphEdit;

    /// <summary>
    /// <para>Button to display the room creation panel.</para>
    /// <para>用于展示房间创建面板的按钮。</para>
    /// </summary>
    private Button? _showCreateRoomPanelButton;

    private PackedScene? _roomNodeScene;
    private Panel? _createOrEditorPanel;
    private Button? _hideCreateRoomPanelButton;
    private LineEdit? _roomNameLineEdit;
    private LineEdit? _roomDescriptionLineEdit;
    private Button? _createRoomButton;

    public override void InitializeData()
    {
        base.InitializeData();
        _roomNodeScene = (PackedScene)GD.Load("res://prefab/ui/RoomNode.tscn");
    }


    public override void InitializeUi()
    {
        base.InitializeUi();
        _graphEdit = GetNode<GraphEdit>("GraphEdit");
        _showCreateRoomPanelButton = GetNode<Button>("ShowCreateRoomPanelButton");
        _createOrEditorPanel = GetNode<Panel>("CreateOrEditorPanel");
        _hideCreateRoomPanelButton = GetNode<Button>("CreateOrEditorPanel/HideCreateRoomPanelButton");
        _roomNameLineEdit = GetNode<LineEdit>("CreateOrEditorPanel/RoomNameLineEdit");
        _roomDescriptionLineEdit = GetNode<LineEdit>("CreateOrEditorPanel/RoomDescriptionLineEdit");
        _createRoomButton = GetNode<Button>("CreateOrEditorPanel/CreateRoomButton");
    }

    /// <summary>
    /// <para>Creating room node</para>
    /// <para>创建房间节点</para>
    /// </summary>
    /// <param name="roomNodeData"></param>
    /// <returns></returns>
    private bool CreateRoomNode(IRoomNodeData roomNodeData)
    {
        if (_roomNodeScene == null || _graphEdit == null)
        {
            return false;
        }

        var node = _roomNodeScene.Instantiate();
        if (node == null)
        {
            return false;
        }

        _graphEdit?.AddChild(node);
        if (node is not RoomNode roomNode)
        {
            return false;
        }

        roomNode.Title = roomNodeData.Title;
        return true;
    }

    public override void LoadUiActions()
    {
        base.LoadUiActions();
        if (_showCreateRoomPanelButton != null)
        {
            _showCreateRoomPanelButton.Pressed += () =>
            {
                if (_graphEdit != null)
                {
                    _graphEdit.Visible = false;
                }

                if (_createOrEditorPanel != null)
                {
                    _createOrEditorPanel.Visible = true;
                }

                _showCreateRoomPanelButton.Visible = false;
            };
        }

        if (_hideCreateRoomPanelButton != null)
        {
            _hideCreateRoomPanelButton.Pressed += HideCreateRoomPanel;
        }

        if (_createRoomButton != null)
        {
            _createRoomButton.Pressed += () =>
            {
                if (_roomNameLineEdit == null || _roomDescriptionLineEdit == null)
                {
                    return;
                }

                var roomNodeData = new RoomNodeData
                {
                    Title = _roomNameLineEdit.Text,
                    Description = _roomDescriptionLineEdit.Text
                };
                var result = CreateRoomNode(roomNodeData);
                if (result)
                {
                    HideCreateRoomPanel();
                }
            };
        }
    }


    /// <summary>
    /// <para>Hide the Create Room panel</para>
    /// <para>隐藏创建房间面板</para>
    /// </summary>
    private void HideCreateRoomPanel()
    {
        if (_graphEdit != null)
        {
            _graphEdit.Visible = true;
        }

        if (_createOrEditorPanel != null)
        {
            _createOrEditorPanel.Visible = false;
        }

        if (_showCreateRoomPanelButton != null)
        {
            _showCreateRoomPanelButton.Visible = true;
        }
    }
}
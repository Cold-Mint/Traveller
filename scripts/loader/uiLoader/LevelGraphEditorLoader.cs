using System.Collections.Generic;
using System.Text.Json;
using ColdMint.scripts.debug;
using ColdMint.scripts.levelGraphEditor;
using ColdMint.scripts.serialization;
using ColdMint.scripts.utils;
using Godot;
using Godot.Collections;

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
    private Button? _returnButton;
    private string? _defaultRoomName;
    private int _roomIndex = 1;
    private TextEdit? _roomTemplateCollectionTextEdit;
    private Label? _roomTemplateTipsLabel;
    private Button? _displayCurrentPageData;

    public override void InitializeData()
    {
        base.InitializeData();
        _roomNodeScene = (PackedScene)GD.Load("res://prefab/ui/RoomNode.tscn");
        _defaultRoomName = TranslationServer.Translate("default_room_name");
    }


    public override void InitializeUi()
    {
        base.InitializeUi();
        _roomTemplateTipsLabel = GetNode<Label>("CreateOrEditorPanel/RoomTemplateTipsLabel");
        if (_roomTemplateTipsLabel != null)
        {
            _roomTemplateTipsLabel.Text = string.Empty;
        }

        _displayCurrentPageData = GetNode<Button>("HBoxContainer/DisplayCurrentPageData");
        _roomTemplateCollectionTextEdit = GetNode<TextEdit>("CreateOrEditorPanel/RoomTemplateCollectionTextEdit");
        _graphEdit = GetNode<GraphEdit>("GraphEdit");
        _showCreateRoomPanelButton = GetNode<Button>("HBoxContainer/ShowCreateRoomPanelButton");
        _returnButton = GetNode<Button>("HBoxContainer/ReturnButton");
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

        roomNode.RoomNodeData = roomNodeData;
        _roomIndex++;
        return true;
    }

    public override void LoadUiActions()
    {
        base.LoadUiActions();
        if (_roomTemplateCollectionTextEdit != null)
        {
            _roomTemplateCollectionTextEdit.TextChanged += () =>
            {
                if (_roomTemplateTipsLabel == null)
                {
                    return;
                }

                var text = _roomTemplateCollectionTextEdit.Text;
                if (string.IsNullOrEmpty(text))
                {
                    _roomTemplateTipsLabel.Text = string.Empty;
                    return;
                }

                var lastLine = StrUtils.GetLastLine(text);
                if (string.IsNullOrEmpty(lastLine))
                {
                    _roomTemplateTipsLabel.Text = string.Empty;
                    return;
                }

                //Parse the last line
                //解析最后一行
                if (lastLine.Length > 0)
                {
                    if (!lastLine.StartsWith("res://"))
                    {
                        var lineError = TranslationServer.Translate("line_errors_must_start_with_res");
                        if (lineError == null)
                        {
                            return;
                        }

                        _roomTemplateTipsLabel.Text = string.Format(lineError, lastLine);
                        return;
                    }

                    var exists = FileAccess.FileExists(lastLine);
                    if (!exists)
                    {
                        var lineError = TranslationServer.Translate("error_specifying_room_template_line");
                        if (lineError == null)
                        {
                            return;
                        }

                        _roomTemplateTipsLabel.Text = string.Format(lineError, lastLine);
                        return;
                    }

                    _roomTemplateTipsLabel.Text = string.Empty;
                }
            };
        }

        if (_graphEdit != null)
        {
            _graphEdit.ConnectionRequest += (fromNode, fromPort, toNode, toPort) =>
            {
                _graphEdit.ConnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
            };
            _graphEdit.DisconnectionRequest += (fromNode, fromPort, toNode, toPort) =>
            {
                _graphEdit.DisconnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
            };
        }

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

                if (_roomNameLineEdit != null && _defaultRoomName != null)
                {
                    _roomNameLineEdit.Text = string.Format(_defaultRoomName, _roomIndex);
                }

                if (_returnButton != null)
                {
                    _returnButton.Visible = false;
                }

                _showCreateRoomPanelButton.Visible = false;
            };
        }

        if (_returnButton != null)
        {
            _returnButton.Pressed += () =>
            {
                GetTree().ChangeSceneToPacked((PackedScene)GD.Load("res://scenes/mainMenu.tscn"));
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

        if (_displayCurrentPageData != null)
        {
            _displayCurrentPageData.Pressed += () =>
            {
                if (_graphEdit == null)
                {
                    return;
                }

                Array<Dictionary> connectionList = _graphEdit.GetConnectionList();
                var levelGraphEditorSaveData = new LevelGraphEditorSaveData();
                var connectionDataList = new List<ConnectionData>();
                levelGraphEditorSaveData.ConnectionData = connectionDataList;
                if (connectionList.Count <= 0) return;
                foreach (var dictionary in connectionList)
                {
                    if (dictionary == null)
                    {
                        continue;
                    }

                    var keys = dictionary.Keys;
                    if (keys.Count == 0)
                    {
                        continue;
                    }

                    var connectionData = new ConnectionData();
                    foreach (var variant in keys)
                    {
                        var typeStr = variant.ToString();
                        switch (typeStr)
                        {
                            case "from_node":
                                // LogCat.Log("查找"+dictionary[variant].Obj);
                                // var obj = dictionary[variant].Obj;
                                // if (obj == null)
                                // {
                                //     LogCat.Log("obj is null");
                                //     continue;
                                // }
                                //
                                // LogCat.Log("obj is " + obj.ToString());
                                // var roomNode = dictionary[variant]. as RoomNode;
                                // LogCat.Log("roomNode is" + (roomNode == null));
                                // var roomNode = dictionary[variant].As<RoomNode>();
                                // LogCat.Log("空的？" + (roomNode == null)+"类为"+dictionary[variant].ToString()+"路径为"+dictionary[variant].AsNodePath()+"对于C#"+dictionary[variant].Obj );
                                // connectionData.From = dictionary[variant].Obj as RoomNode;
                                // LogCat.Log("类型" + dictionary[variant]);
                                break;
                            case "from_port":
                                connectionData.FromPort = dictionary[variant].AsInt32();
                                break;
                            case "to_node":
                                connectionData.To = dictionary[variant].Obj as IRoomNodeData;
                                break;
                            case "to_port":
                                connectionData.ToPort = dictionary[variant].AsInt32();
                                break;
                        }

                        LogCat.Log(variant.ToString());
                        connectionDataList.Add(connectionData);
                    }
                }

                LogCat.Log(JsonSerialization.Serialize(connectionDataList));
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

        if (_returnButton != null)
        {
            _returnButton.Visible = true;
        }

        if (_showCreateRoomPanelButton != null)
        {
            _showCreateRoomPanelButton.Visible = true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using ColdMint.scripts.debug;
using ColdMint.scripts.levelGraphEditor;
using ColdMint.scripts.serialization;
using ColdMint.scripts.utils;
using Godot;
using Godot.Collections;
using FileAccess = Godot.FileAccess;

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

    /// <summary>
    /// <para>Index of the room</para>
    /// <para>房间的索引</para>
    /// </summary>
    private int _roomIndex = 1;

    private TextEdit? _roomTemplateCollectionTextEdit;
    private Label? _roomTemplateTipsLabel;
    private Button? _showSavePanelButton;
    private Button? _openExportFolderButton;
    private HBoxContainer? _hBoxContainer;
    private Panel? _saveOrLoadPanel;
    private Button? _cancelButton;
    private Button? _actionButton;
    private Label? _saveOrLoadPanelTitleLabel;
    private LineEdit? _fileNameLineEdit;
    private Button? _showLoadPanelButton;
    private Button? _deleteSelectedNodeButton;
    private readonly List<Node> _selectedNodes = new List<Node>();

    /// <summary>
    /// <para>Displays the time to enter the suggestion</para>
    /// <para>显示输入建议的时刻</para>
    /// </summary>
    private DateTime? _displaysTheSuggestedInputTime;

    /// <summary>
    /// <para>Offset to append when a new node is created.</para>
    /// <para>创建新节点时追加的偏移量。</para>
    /// </summary>
    private Vector2 _positionOffset = new Vector2(100, 100);

    /// <summary>
    /// <para>Is the press event of an active button saved?</para>
    /// <para>活动按钮的按下事件是否为保存？</para>
    /// </summary>
    private bool _saveMode;


    public override void InitializeData()
    {
        base.InitializeData();
        _roomNodeScene = (PackedScene)GD.Load("res://prefab/ui/RoomNode.tscn");
        _defaultRoomName = TranslationServer.Translate("default_room_name");
        var folder = Config.GetLevelGraphExportDirectory();
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
    }


    public override void InitializeUi()
    {
        base.InitializeUi();
        _roomTemplateTipsLabel = GetNode<Label>("CreateOrEditorPanel/RoomTemplateTipsLabel");
        if (_roomTemplateTipsLabel != null)
        {
            _roomTemplateTipsLabel.Text = string.Empty;
        }

        _openExportFolderButton = GetNode<Button>("HBoxContainer/OpenExportFolderButton");
        if (_openExportFolderButton != null)
        {
            //If open directories are supported, a button is displayed.
            //若支持打开目录，那么显示按钮。
            _openExportFolderButton.Visible = ExplorerUtils.SupportOpenDirectory();
        }

        _showLoadPanelButton = GetNode<Button>("HBoxContainer/ShowLoadPanelButton");
        _saveOrLoadPanelTitleLabel = GetNode<Label>("SaveOrLoadPanel/SaveOrLoadPanelTitleLabel");
        _saveOrLoadPanel = GetNode<Panel>("SaveOrLoadPanel");
        _fileNameLineEdit = GetNode<LineEdit>("SaveOrLoadPanel/FileNameLineEdit");
        _actionButton = GetNode<Button>("SaveOrLoadPanel/HBoxContainer/ActionButton");
        _cancelButton = GetNode<Button>("SaveOrLoadPanel/HBoxContainer/CancelButton");
        _hBoxContainer = GetNode<HBoxContainer>("HBoxContainer");
        _showSavePanelButton = GetNode<Button>("HBoxContainer/ShowSavePanelButton");
        _roomTemplateCollectionTextEdit = GetNode<TextEdit>("CreateOrEditorPanel/RoomTemplateCollectionTextEdit");
        _graphEdit = GetNode<GraphEdit>("GraphEdit");
        _deleteSelectedNodeButton = GetNode<Button>("HBoxContainer/DeleteSelectedNodeButton");
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
    private RoomNode? CreateRoomNode(RoomNodeData roomNodeData)
    {
        if (_roomNodeScene == null || _graphEdit == null)
        {
            return null;
        }

        var node = _roomNodeScene.Instantiate();
        if (node == null)
        {
            return null;
        }

        _graphEdit?.AddChild(node);
        if (node is not RoomNode roomNode)
        {
            return null;
        }

        roomNode.RoomNodeData = roomNodeData;
        roomNode.PositionOffset = _positionOffset * _roomIndex;
        _roomIndex++;
        return roomNode;
    }

    /// <summary>
    /// <para>Displays input suggestions for room templates</para>
    /// <para>显示房间模板的输入建议</para>
    /// </summary>
    private void DisplayInputPrompt()
    {
        if (_roomTemplateTipsLabel == null || _roomTemplateCollectionTextEdit == null)
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

            var fileExists = FileAccess.FileExists(lastLine);
            var dirExists = DirAccess.DirExistsAbsolute(lastLine);
            if (!fileExists && !dirExists)
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
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_displaysTheSuggestedInputTime != null && DateTime.Now > _displaysTheSuggestedInputTime)
        {
            //Performs the actual input field text change event.
            //执行实际的输入框文本改变事件。
            DisplayInputPrompt();
            _displaysTheSuggestedInputTime = null;
        }
    }

    public override void LoadUiActions()
    {
        base.LoadUiActions();
        if (_roomTemplateCollectionTextEdit != null)
        {
            _roomTemplateCollectionTextEdit.TextChanged += () =>
            {
                //Add anti-shake treatment.
                //添加防抖处理。
                //Higher frequency events are executed last time.
                //频率较高的事件中，执行最后一次。
                _displaysTheSuggestedInputTime = DateTime.Now.Add(TimeSpan.FromMilliseconds(Config.TextChangesBuffetingDuration));
            };
        }

        if (_graphEdit != null)
        {
            _graphEdit.NodeSelected += node => { _selectedNodes.Add(node); };
            _graphEdit.NodeDeselected += node => { _selectedNodes.Remove(node); };
            _graphEdit.ConnectionRequest += (fromNode, fromPort, toNode, toPort) =>
            {
                _graphEdit.ConnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
            };
            _graphEdit.DisconnectionRequest += (fromNode, fromPort, toNode, toPort) =>
            {
                _graphEdit.DisconnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
            };
        }

        if (_openExportFolderButton != null)
        {
            _openExportFolderButton.Pressed += () =>
            {
                ExplorerUtils.OpenFolder(Config.GetLevelGraphExportDirectory());
            };
        }

        if (_deleteSelectedNodeButton != null)
        {
            _deleteSelectedNodeButton.Pressed += () =>
            {
                if (_graphEdit == null)
                {
                    return;
                }

                if (_selectedNodes.Count == 0)
                {
                    return;
                }

                var nodes = _selectedNodes.ToArray();
                foreach (var node in nodes)
                {
                    if (node is not RoomNode roomNode)
                    {
                        continue;
                    }

                    _graphEdit.RemoveChild(node);
                    roomNode.QueueFree();
                    _selectedNodes.Remove(node);
                }
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

                if (_hBoxContainer != null)
                {
                    _hBoxContainer.Visible = false;
                }
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
                if (_roomNameLineEdit == null || _roomDescriptionLineEdit == null ||
                    _roomTemplateCollectionTextEdit == null)
                {
                    return;
                }

                var roomTemplateData = _roomTemplateCollectionTextEdit.Text;
                if (string.IsNullOrEmpty(roomTemplateData))
                {
                    return;
                }

                var roomTemplateArray = roomTemplateData.Split('\n');
                if (roomTemplateArray.Length == 0)
                {
                    return;
                }

                var roomNodeData = new RoomNodeData
                {
                    Id = GuidUtils.GetGuid(),
                    Title = _roomNameLineEdit.Text,
                    Description = _roomDescriptionLineEdit.Text,
                    RoomTemplateSet = roomTemplateArray
                };
                var roomNode = CreateRoomNode(roomNodeData);
                if (roomNode != null)
                {
                    HideCreateRoomPanel();
                }
            };
        }

        if (_cancelButton != null)
        {
            _cancelButton.Pressed += () =>
            {
                if (_saveOrLoadPanel != null)
                {
                    _saveOrLoadPanel.Visible = false;
                }
            };
        }

        if (_actionButton != null)
        {
            _actionButton.Pressed += () =>
            {
                if (_saveOrLoadPanel != null)
                {
                    _saveOrLoadPanel.Visible = false;
                }
            };
        }

        if (_showLoadPanelButton != null)
        {
            _showLoadPanelButton.Pressed += () =>
            {
                if (_saveOrLoadPanel != null)
                {
                    _saveOrLoadPanel.Visible = true;
                }

                if (_actionButton != null)
                {
                    _actionButton.Text = TranslationServer.Translate("load");
                }

                if (_fileNameLineEdit != null)
                {
                    _fileNameLineEdit.Text = string.Empty;
                }

                if (_saveOrLoadPanelTitleLabel != null)
                {
                    _saveOrLoadPanelTitleLabel.Text = TranslationServer.Translate("load");
                }

                _saveMode = false;
            };
        }

        if (_showSavePanelButton != null)
        {
            _showSavePanelButton.Pressed += () =>
            {
                if (_saveOrLoadPanel != null)
                {
                    _saveOrLoadPanel.Visible = true;
                }

                if (_actionButton != null)
                {
                    _actionButton.Text = TranslationServer.Translate("save");
                }

                if (_fileNameLineEdit != null)
                {
                    _fileNameLineEdit.Text = string.Empty;
                }

                if (_saveOrLoadPanelTitleLabel != null)
                {
                    _saveOrLoadPanelTitleLabel.Text = TranslationServer.Translate("save");
                }

                _saveMode = true;
            };
        }

        if (_actionButton != null)
        {
            _actionButton.Pressed += () =>
            {
                if (_fileNameLineEdit == null)
                {
                    return;
                }

                var fileName = _fileNameLineEdit.Text;
                if (string.IsNullOrEmpty(fileName))
                {
                    return;
                }

                if (_saveMode)
                {
                    SaveFile(fileName);
                }

                else
                {
                    LoadFile(fileName);
                }
            };
        }
    }


    /// <summary>
    /// <para>save file</para>
    /// <para>保存文件</para>
    /// </summary>
    /// <param name="fileName">
    ///<para>filename</para>
    ///<para>文件名</para>
    /// </param>
    private async void SaveFile(string fileName)
    {
        if (_graphEdit == null)
        {
            return;
        }

        var levelGraphEditorSaveData = new LevelGraphEditorSaveData();
        //Serialize room node information
        //序列化房间节点信息
        var length = _graphEdit.GetChildCount();
        if (length <= 0)
        {
            //no room
            //没有房间
            return;
        }

        var roomNodeDataList = new List<RoomNodeData>();
        levelGraphEditorSaveData.RoomNodeDataList = roomNodeDataList;
        for (var i = 0; i < length; i++)
        {
            var node = _graphEdit.GetChild(i);
            if (node is not RoomNode roomNode) continue;
            var data = roomNode.RoomNodeData;
            if (data == null)
            {
                continue;
            }

            roomNodeDataList.Add(data);
        }

        //Serialized connection information
        //序列化连接信息
        Array<Dictionary> connectionList = _graphEdit.GetConnectionList();
        var connectionDataList = new List<ConnectionData>();
        levelGraphEditorSaveData.ConnectionDataList = connectionDataList;
        if (connectionList.Count > 0)
        {
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
                            var fromRoomNodeData = GetRoomNodeData(dictionary[variant].AsString());
                            if (fromRoomNodeData == null)
                            {
                                continue;
                            }

                            connectionData.FromId = fromRoomNodeData.Id;
                            break;
                        case "from_port":
                            connectionData.FromPort = dictionary[variant].AsInt32();
                            break;
                        case "to_node":
                            var toRoomNodeData = GetRoomNodeData(dictionary[variant].AsString());
                            if (toRoomNodeData == null)
                            {
                                continue;
                            }

                            connectionData.ToId = toRoomNodeData.Id;
                            break;
                        case "to_port":
                            connectionData.ToPort = dictionary[variant].AsInt32();
                            break;
                    }
                }

                connectionDataList.Add(connectionData);
            }
        }

        var filePath = Path.Join(Config.GetLevelGraphExportDirectory(), FileNameToActualName(fileName));
        await File.WriteAllTextAsync(filePath, JsonSerialization.Serialize(levelGraphEditorSaveData));
    }

    /// <summary>
    /// <para>Filename to real name</para>
    /// <para>文件名到真实名称</para>
    /// </summary>
    /// <param name="fileName">
    ///<para>If the filename does not have a file format, add the file format.</para>
    ///<para>若文件名没有文件格式，那么加上文件格式。</para>
    /// </param>
    /// <returns></returns>
    private string FileNameToActualName(string fileName)
    {
        string actualName;
        if (fileName.EndsWith(".json"))
        {
            actualName = fileName;
        }
        else
        {
            actualName = fileName + ".json";
        }

        return actualName;
    }


    private async void LoadFile(string fileName)
    {
        if (_graphEdit == null)
        {
            return;
        }

        var filePath = Path.Join(Config.GetLevelGraphExportDirectory(), FileNameToActualName(fileName));
        if (!File.Exists(filePath))
        {
            //file does not exist
            //文件不存在。
            return;
        }

        var levelGraphEditorSaveData =
            await JsonSerialization.ReadJsonFileToObj<LevelGraphEditorSaveData>(filePath);
        if (levelGraphEditorSaveData == null)
        {
            //Deserialization failed.
            //反序列化失败。
            return;
        }

        //Do not call DeleteAllChildAsync; this will raise "ERROR: Caller thread can't call this function in this node."
        //不要调用DeleteAllChildAsync方法，这会引发“ERROR: Caller thread can't call this function in this node”。
        NodeUtils.DeleteAllChild(_graphEdit);
        _roomIndex = 1;
        var roomNodeDataList = levelGraphEditorSaveData.RoomNodeDataList;
        if (roomNodeDataList != null)
        {
            foreach (var roomNodeData in roomNodeDataList)
            {
                if (string.IsNullOrEmpty(roomNodeData.Id))
                {
                    continue;
                }

                //Instantiate the room node.
                //实例化房间节点。
                var roomNode = CreateRoomNode(roomNodeData);
                if (roomNode != null)
                {
                    roomNode.Name = roomNodeData.Id;
                }
            }
        }

        var connectionDataList = levelGraphEditorSaveData.ConnectionDataList;
        if (connectionDataList != null)
        {
            foreach (var connectionData in connectionDataList)
            {
                if (string.IsNullOrEmpty(connectionData.FromId))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(connectionData.ToId))
                {
                    continue;
                }

                //Connecting rooms
                //连接房间
                _graphEdit.ConnectNode(connectionData.FromId, connectionData.FromPort, connectionData.ToId,
                    connectionData.ToPort);
            }
        }
    }

    /// <summary>
    /// <para>Get node data by name</para>
    /// <para>根据名称获取节点数据</para>
    /// </summary>
    /// <param name="name">
    ///<para>name</para>
    ///<para>名称</para>
    /// </param>
    /// <returns></returns>
    private RoomNodeData? GetRoomNodeData(string name)
    {
        if (_graphEdit == null)
        {
            return null;
        }

        var roomNode = _graphEdit.GetNodeOrNull<RoomNode>(name);
        if (roomNode == null)
        {
            return null;
        }

        return roomNode.RoomNodeData;
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

        if (_hBoxContainer != null)
        {
            _hBoxContainer.Visible = true;
        }
    }
}
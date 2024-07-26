using System;
using System.Collections.Generic;
using System.IO;
using ColdMint.scripts.levelGraphEditor;
using ColdMint.scripts.nodeBinding;
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
    private string? _defaultRoomName;

    private readonly LevelGraphEditorBinding _nodeBinding = new();

    /// <summary>
    /// <para>Index of the room</para>
    /// <para>房间的索引</para>
    /// </summary>
    private int _roomIndex = 1;

    /// <summary>
    /// <para>Is there a start node?</para>
    /// <para>是否有开始节点了？</para>
    /// </summary>
    private bool _hasStartNode;

    private PackedScene? _roomNodeScene;

    private readonly List<Node> _selectedNodes = new();
    private PackedScene? _mainMenu;

    /// <summary>
    /// <para>Displays the time to enter the suggestion</para>
    /// <para>显示输入建议的时刻</para>
    /// </summary>
    private DateTime? _displaysTheSuggestedInputTime;

    /// <summary>
    /// <para>Offset to append when a new node is created.</para>
    /// <para>创建新节点时追加的偏移量。</para>
    /// </summary>
    private Vector2 _positionOffset = new(100, 100);

    /// <summary>
    /// <para>Is the press event of an active button saved?</para>
    /// <para>活动按钮的按下事件是否为保存？</para>
    /// </summary>
    private bool _saveMode;


    public override void InitializeData()
    {
        base.InitializeData();
        _mainMenu = GD.Load<PackedScene>("res://scenes/mainMenu.tscn");
        _roomNodeScene = GD.Load<PackedScene>("res://prefab/ui/RoomNode.tscn");
        _defaultRoomName = TranslationServerUtils.Translate("ui_default_room_name");
        var folder = Config.GetLevelGraphExportDirectory();
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
    }


    public override void InitializeUi()
    {
        base.InitializeUi();
        _nodeBinding.Binding(this);
        if (_nodeBinding.OpenExportFolderButton != null)
        {
            //If open directories are supported, a button is displayed.
            //若支持打开目录，那么显示按钮。
            _nodeBinding.OpenExportFolderButton.Visible = ExplorerUtils.SupportOpenDirectory();
        }

        if (_nodeBinding.RoomTemplateTipsLabel != null)
        {
            _nodeBinding.RoomTemplateTipsLabel.Text = string.Empty;
        }
    }

    /// <summary>
    /// <para>Creating room node</para>
    /// <para>创建房间节点</para>
    /// </summary>
    /// <param name="roomNodeData"></param>
    /// <returns></returns>
    private RoomNode? CreateRoomNode(RoomNodeData roomNodeData)
    {
        if (_roomNodeScene == null || _nodeBinding.GraphEdit == null)
        {
            return null;
        }

        var node = NodeUtils.InstantiatePackedScene<Node>(_roomNodeScene);
        if (node == null)
        {
            return null;
        }

        _nodeBinding.GraphEdit?.AddChild(node);
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
        if (_nodeBinding.RoomTemplateTipsLabel == null || _nodeBinding.RoomTemplateCollectionTextEdit == null)
        {
            return;
        }

        var text = _nodeBinding.RoomTemplateCollectionTextEdit.Text;
        if (string.IsNullOrEmpty(text))
        {
            _nodeBinding.RoomTemplateTipsLabel.Text = string.Empty;
            return;
        }

        var lastLine = StrUtils.GetLastLine(text);
        if (string.IsNullOrEmpty(lastLine))
        {
            _nodeBinding.RoomTemplateTipsLabel.Text = string.Empty;
            return;
        }

        //Parse the last line
        //解析最后一行
        if (lastLine.Length > 0)
        {
            if (!lastLine.StartsWith("res://"))
            {
                var lineError = TranslationServer.Translate("ui_line_errors_must_start_with_res");
                if (lineError == null)
                {
                    return;
                }

                _nodeBinding.RoomTemplateTipsLabel.Text = string.Format(lineError, lastLine);
                return;
            }

            var fileExists = FileAccess.FileExists(lastLine);
            var dirExists = DirAccess.DirExistsAbsolute(lastLine);
            if (!fileExists && !dirExists)
            {
                var lineError = TranslationServerUtils.Translate("ui_error_specifying_room_template_line");
                if (lineError == null)
                {
                    return;
                }

                _nodeBinding.RoomTemplateTipsLabel.Text = string.Format(lineError, lastLine);
                return;
            }

            _nodeBinding.RoomTemplateTipsLabel.Text = string.Empty;
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
        if (_nodeBinding.RoomTemplateCollectionTextEdit != null)
        {
            _nodeBinding.RoomTemplateCollectionTextEdit.TextChanged += () =>
            {
                //Add anti-shake treatment.
                //添加防抖处理。
                //Higher frequency events are executed last time.
                //频率较高的事件中，执行最后一次。
                _displaysTheSuggestedInputTime =
                    DateTime.Now.Add(TimeSpan.FromMilliseconds(Config.TextChangesBuffetingDuration));
            };
        }

        if (_nodeBinding.GraphEdit != null)
        {
            _nodeBinding.GraphEdit.NodeSelected += node => { _selectedNodes.Add(node); };
            _nodeBinding.GraphEdit.NodeDeselected += node => { _selectedNodes.Remove(node); };
            _nodeBinding.GraphEdit.ConnectionRequest += (fromNode, fromPort, toNode, toPort) =>
            {
                _nodeBinding.GraphEdit.ConnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
            };
            _nodeBinding.GraphEdit.DisconnectionRequest += (fromNode, fromPort, toNode, toPort) =>
            {
                _nodeBinding.GraphEdit.DisconnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
            };
        }

        if (_nodeBinding.OpenExportFolderButton != null)
        {
            _nodeBinding.OpenExportFolderButton.Pressed += () =>
            {
                ExplorerUtils.OpenFolder(Config.GetLevelGraphExportDirectory());
            };
        }

        if (_nodeBinding.DeleteSelectedNodeButton != null)
        {
            _nodeBinding.DeleteSelectedNodeButton.Pressed += () =>
            {
                if (_nodeBinding.GraphEdit == null)
                {
                    return;
                }

                if (_selectedNodes.Count == 0)
                {
                    return;
                }

                var nodes = _selectedNodes.ToArray();
                _roomIndex -= _selectedNodes.Count;
                foreach (var node in nodes)
                {
                    if (node is not RoomNode roomNode)
                    {
                        continue;
                    }

                    if (roomNode.RoomNodeData != null && roomNode.RoomNodeData.HasTag(Config.RoomDataTag.StartingRoom))
                    {
                        //The node with the start room label was deleted.
                        //删除了带有起始房间标签的节点。
                        _hasStartNode = false;
                    }

                    _nodeBinding.GraphEdit.RemoveChild(node);
                    roomNode.QueueFree();
                    _selectedNodes.Remove(node);
                }
            };
        }

        if (_nodeBinding.ShowCreateRoomPanelButton != null)
        {
            _nodeBinding.ShowCreateRoomPanelButton.Pressed += () =>
            {
                if (_nodeBinding.GraphEdit != null)
                {
                    _nodeBinding.GraphEdit.Hide();
                }

                if (_nodeBinding.CreateOrEditorPanel != null)
                {
                    _nodeBinding.CreateOrEditorPanel.Show();
                }

                if (_nodeBinding.RoomNameLineEdit != null && _defaultRoomName != null)
                {
                    _nodeBinding.RoomNameLineEdit.Text = string.Format(_defaultRoomName, _roomIndex);
                }

                if (_nodeBinding.RoomDescriptionLineEdit != null)
                {
                    _nodeBinding.RoomDescriptionLineEdit.Text = string.Empty;
                }

                if (_nodeBinding.TagLineEdit != null)
                {
                    if (_hasStartNode)
                    {
                        _nodeBinding.TagLineEdit.Text = string.Empty;
                    }
                    else
                    {
                        _nodeBinding.TagLineEdit.Text = Config.RoomDataTag.StartingRoom;
                    }
                }

                if (_nodeBinding.HBoxContainer != null)
                {
                    _nodeBinding.HBoxContainer.Hide();
                }
            };
        }

        if (_nodeBinding.ExitButton != null)
        {
            _nodeBinding.ExitButton.Pressed += () =>
            {
                if (_mainMenu == null)
                {
                    return;
                }
                GetTree().ChangeSceneToPacked(_mainMenu);
            };
        }

        if (_nodeBinding.HideCreateRoomPanelButton != null)
        {
            _nodeBinding.HideCreateRoomPanelButton.Pressed += HideCreateRoomPanel;
        }

        if (_nodeBinding.CreateRoomButton != null)
        {
            _nodeBinding.CreateRoomButton.Pressed += () =>
            {
                if (_nodeBinding.RoomNameLineEdit == null || _nodeBinding.RoomDescriptionLineEdit == null ||
                    _nodeBinding.RoomTemplateCollectionTextEdit == null || _nodeBinding.TagLineEdit == null)
                {
                    return;
                }

                var roomTemplateData = _nodeBinding.RoomTemplateCollectionTextEdit.Text;
                if (string.IsNullOrEmpty(roomTemplateData))
                {
                    return;
                }

                var roomTemplateArray = roomTemplateData.Split('\n');
                if (roomTemplateArray.Length == 0)
                {
                    return;
                }

                string[]? tagArray = null;
                var tagData = _nodeBinding.TagLineEdit.Text;
                if (!string.IsNullOrEmpty(tagData))
                {
                    tagArray = tagData.Split(',');
                }


                var roomNodeData = new RoomNodeData
                {
                    Id = GuidUtils.GetGuid(),
                    Title = _nodeBinding.RoomNameLineEdit.Text,
                    Description = _nodeBinding.RoomDescriptionLineEdit.Text,
                    RoomTemplateSet = roomTemplateArray,
                    Tags = tagArray,
                    RoomInjectionProcessorData = _nodeBinding.RoomInjectionProcessorDataTextEdit?.Text
                };
                if (!_hasStartNode && roomNodeData.HasTag(Config.RoomDataTag.StartingRoom))
                {
                    _hasStartNode = true;
                }

                var roomNode = CreateRoomNode(roomNodeData);
                if (roomNode != null)
                {
                    HideCreateRoomPanel();
                }
            };
        }

        if (_nodeBinding.CancelButton != null)
        {
            _nodeBinding.CancelButton.Pressed += () =>
            {
                if (_nodeBinding.SaveOrLoadPanel != null)
                {
                    _nodeBinding.SaveOrLoadPanel.Hide();
                }
            };
        }

        if (_nodeBinding.ActionButton != null)
        {
            _nodeBinding.ActionButton.Pressed += () =>
            {
                if (_nodeBinding.SaveOrLoadPanel != null)
                {
                    _nodeBinding.SaveOrLoadPanel.Hide();
                }
            };
        }

        if (_nodeBinding.ShowLoadPanelButton != null)
        {
            _nodeBinding.ShowLoadPanelButton.Pressed += () =>
            {
                if (_nodeBinding.SaveOrLoadPanel != null)
                {
                    _nodeBinding.SaveOrLoadPanel.Show();
                }

                if (_nodeBinding.ActionButton != null)
                {
                    _nodeBinding.ActionButton.Text = TranslationServerUtils.Translate("ui_load");
                }

                if (_nodeBinding.FileNameLineEdit != null)
                {
                    _nodeBinding.FileNameLineEdit.Text = string.Empty;
                }

                if (_nodeBinding.SaveOrLoadPanelTitleLabel != null)
                {
                    _nodeBinding.SaveOrLoadPanelTitleLabel.Text = TranslationServerUtils.Translate("ui_load");
                }

                _saveMode = false;
            };
        }

        if (_nodeBinding.ShowSavePanelButton != null)
        {
            _nodeBinding.ShowSavePanelButton.Pressed += () =>
            {
                if (_nodeBinding.SaveOrLoadPanel != null)
                {
                    _nodeBinding.SaveOrLoadPanel.Show();
                }

                if (_nodeBinding.ActionButton != null)
                {
                    _nodeBinding.ActionButton.Text = TranslationServerUtils.Translate("ui_save");
                }

                if (_nodeBinding.FileNameLineEdit != null)
                {
                    _nodeBinding.FileNameLineEdit.Text = string.Empty;
                }

                if (_nodeBinding.SaveOrLoadPanelTitleLabel != null)
                {
                    _nodeBinding.SaveOrLoadPanelTitleLabel.Text = TranslationServerUtils.Translate("ui_save");
                }

                _saveMode = true;
            };
        }

        if (_nodeBinding.ActionButton != null)
        {
            _nodeBinding.ActionButton.Pressed += () =>
            {
                if (_nodeBinding.FileNameLineEdit == null)
                {
                    return;
                }

                var fileName = _nodeBinding.FileNameLineEdit.Text;
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
        if (_nodeBinding.GraphEdit == null)
        {
            return;
        }

        var levelGraphEditorSaveData = new LevelGraphEditorSaveData();
        //Serialize room node information
        //序列化房间节点信息
        var length = _nodeBinding.GraphEdit.GetChildCount();
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
            var node = _nodeBinding.GraphEdit.GetChild(i);
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
        Array<Dictionary> connectionList = _nodeBinding.GraphEdit.GetConnectionList();
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
        await File.WriteAllTextAsync(filePath, YamlSerialization.Serialize(levelGraphEditorSaveData));
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
        if (fileName.EndsWith(".yaml"))
        {
            actualName = fileName;
        }
        else
        {
            actualName = fileName + ".yaml";
        }

        return actualName;
    }


    private async void LoadFile(string fileName)
    {
        if (_nodeBinding.GraphEdit == null)
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
            await YamlSerialization.ReadYamlFileToObj<LevelGraphEditorSaveData>(filePath);
        if (levelGraphEditorSaveData == null)
        {
            //Deserialization failed.
            //反序列化失败。
            return;
        }

        //Do not call DeleteAllChildAsync; this will raise "ERROR: Caller thread can't call this function in this node."
        //不要调用DeleteAllChildAsync方法，这会引发“ERROR: Caller thread can't call this function in this node”。
        NodeUtils.DeleteAllChild(_nodeBinding.GraphEdit);
        _roomIndex = 1;
        _hasStartNode = false;
        var roomNodeDataList = levelGraphEditorSaveData.RoomNodeDataList;
        if (roomNodeDataList != null)
        {
            foreach (var roomNodeData in roomNodeDataList)
            {
                if (string.IsNullOrEmpty(roomNodeData.Id))
                {
                    continue;
                }

                if (!_hasStartNode && roomNodeData.HasTag(Config.RoomDataTag.StartingRoom))
                {
                    _hasStartNode = true;
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
                _nodeBinding.GraphEdit?.ConnectNode(connectionData.FromId, connectionData.FromPort, connectionData.ToId,
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
        if (_nodeBinding.GraphEdit == null)
        {
            return null;
        }

        var roomNode = _nodeBinding.GraphEdit.GetNodeOrNull<RoomNode>(name);
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
        if (_nodeBinding.GraphEdit != null)
        {
            _nodeBinding.GraphEdit.Show();
        }

        if (_nodeBinding.CreateOrEditorPanel != null)
        {
            _nodeBinding.CreateOrEditorPanel.Hide();
        }

        if (_nodeBinding.HBoxContainer != null)
        {
            _nodeBinding.HBoxContainer.Show();
        }
    }
}

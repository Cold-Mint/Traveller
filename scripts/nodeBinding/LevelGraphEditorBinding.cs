using Godot;

namespace ColdMint.scripts.nodeBinding;

public class LevelGraphEditorBinding : INodeBinding
{
    public GraphEdit? GraphEdit;

    /// <summary>
    /// <para>Button to display the room creation panel.</para>
    /// <para>用于展示房间创建面板的按钮。</para>
    /// </summary>
    public Button? ShowCreateRoomPanelButton;

    public Panel? CreateOrEditorPanel;
    public Button? HideCreateRoomPanelButton;
    public LineEdit? RoomNameLineEdit;
    public LineEdit? RoomDescriptionLineEdit;
    public Button? CreateRoomButton;
    public Button? ReturnButton;
    public TextEdit? RoomTemplateCollectionTextEdit;
    public Label? RoomTemplateTipsLabel;
    public Button? ShowSavePanelButton;
    public Button? OpenExportFolderButton;
    public HBoxContainer? HBoxContainer;
    public Panel? SaveOrLoadPanel;
    public Button? CancelButton;
    public Button? ActionButton;
    public Label? SaveOrLoadPanelTitleLabel;
    public LineEdit? FileNameLineEdit;
    public Button? ShowLoadPanelButton;
    public Button? DeleteSelectedNodeButton;
    public LineEdit? TagLineEdit;
    public TextEdit? RoomInjectionProcessorDataTextEdit;
    public void Binding(Node root)
    {
        RoomTemplateTipsLabel = root.GetNode<Label>("CreateOrEditorPanel/RoomTemplateTipsLabel");
        OpenExportFolderButton = root.GetNode<Button>("HBoxContainer/OpenExportFolderButton");
        ShowLoadPanelButton = root.GetNode<Button>("HBoxContainer/ShowLoadPanelButton");
        SaveOrLoadPanelTitleLabel = root.GetNode<Label>("SaveOrLoadPanel/SaveOrLoadPanelTitleLabel");
        SaveOrLoadPanel = root.GetNode<Panel>("SaveOrLoadPanel");
        FileNameLineEdit = root.GetNode<LineEdit>("SaveOrLoadPanel/FileNameLineEdit");
        ActionButton = root.GetNode<Button>("SaveOrLoadPanel/HBoxContainer/ActionButton");
        CancelButton = root.GetNode<Button>("SaveOrLoadPanel/HBoxContainer/CancelButton");
        HBoxContainer = root.GetNode<HBoxContainer>("HBoxContainer");
        ShowSavePanelButton = root.GetNode<Button>("HBoxContainer/ShowSavePanelButton");
        RoomTemplateCollectionTextEdit = root.GetNode<TextEdit>("CreateOrEditorPanel/RoomTemplateCollectionTextEdit");
        GraphEdit = root.GetNode<GraphEdit>("GraphEdit");
        DeleteSelectedNodeButton = root.GetNode<Button>("HBoxContainer/DeleteSelectedNodeButton");
        ShowCreateRoomPanelButton = root.GetNode<Button>("HBoxContainer/ShowCreateRoomPanelButton");
        ReturnButton = root.GetNode<Button>("HBoxContainer/ReturnButton");
        CreateOrEditorPanel = root.GetNode<Panel>("CreateOrEditorPanel");
        HideCreateRoomPanelButton = root.GetNode<Button>("CreateOrEditorPanel/HideCreateRoomPanelButton");
        RoomNameLineEdit = root.GetNode<LineEdit>("CreateOrEditorPanel/RoomNameLineEdit");
        RoomDescriptionLineEdit = root.GetNode<LineEdit>("CreateOrEditorPanel/RoomDescriptionLineEdit");
        CreateRoomButton = root.GetNode<Button>("CreateOrEditorPanel/CreateRoomButton");
        TagLineEdit = root.GetNode<LineEdit>("CreateOrEditorPanel/TagLineEdit");
        RoomInjectionProcessorDataTextEdit = root.GetNode<TextEdit>("CreateOrEditorPanel/RoomInjectionProcessorDataTextEdit");
    }
}
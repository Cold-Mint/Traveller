namespace ColdMint.scripts.levelGraphEditor;

public class ConnectionData
{
    public IRoomNodeData? From { get; set; }
    public IRoomNodeData? To { get; set; }
    public int FromPort { get; set; }
    public int ToPort { get; set; }
}
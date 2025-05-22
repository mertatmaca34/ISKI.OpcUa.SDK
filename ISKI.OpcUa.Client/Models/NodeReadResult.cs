namespace ISKI.OpcUa.Client.Models;

public class NodeReadResult
{
    public string NodeId { get; set; } = string.Empty;
    public object? Value { get; set; }
    public string NodeStatus { get; set; } = "Unknown";
}

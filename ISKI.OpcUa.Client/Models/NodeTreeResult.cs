namespace ISKI.OpcUa.Client.Models;

public class NodeTreeResult
{
    public string DisplayName { get; set; } = string.Empty;
    public string NodeId { get; set; } = string.Empty;
    public string NodeClass { get; set; } = string.Empty;
    public List<NodeTreeResult> Children { get; set; } = new();
}

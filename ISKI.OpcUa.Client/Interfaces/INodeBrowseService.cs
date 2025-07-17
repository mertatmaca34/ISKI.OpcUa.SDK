using ISKI.OpcUa.Client.Models;

namespace ISKI.OpcUa.Client.Interfaces;

public interface INodeBrowseService
{
    List<NodeBrowseResult> Browse(string nodeId);
    NodeTreeResult BrowseTree(string nodeId);
}

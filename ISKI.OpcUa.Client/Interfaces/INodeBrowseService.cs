namespace ISKI.OpcUa.Client.Interfaces;

public interface INodeBrowseService
{
    List<string> Browse(string nodeId);
}

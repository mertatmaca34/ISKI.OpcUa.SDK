using ISKI.OpcUa.Client.Interfaces;
using ISKI.OpcUa.Client.Models;

namespace ISKI.OpcUa.Client.Services;

public class OpcUaService(
    IConnectionService connection,
    INodeReadWriteService readWrite,
    INodeBrowseService browser,
    IDiscoveryService discovery) : IOpcUaService
{
    public Task ConnectAsync(string endpointUrl) => connection.ConnectAsync(endpointUrl);
    public Task DisconnectAsync() => connection.DisconnectAsync();
    public Task<ConnectionResult<NodeReadResult>> ReadNodeAsync(string nodeId) => readWrite.ReadNodeAsync(nodeId);
    public Task WriteNodeAsync(string nodeId, object value, CancellationToken ct) => readWrite.WriteNodeAsync(nodeId, value, ct);
    public List<NodeBrowseResult> Browse(string nodeId) => browser.Browse(nodeId);
    public NodeTreeResult BrowseTree(string nodeId) => browser.BrowseTree(nodeId);
    public Task<List<string>> FindServersOnLocalNetworkAsync() => discovery.FindServersOnLocalNetworkAsync();
    public Task<List<string>> FindServersOnLocalNetworkAsync(string networkPrefix = "192.168.1", int port = 4840) => discovery.FindServersOnLocalNetworkAsync(networkPrefix, port);
}
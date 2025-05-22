using ISKI.OpcUa.Client.Interfaces;
using ISKI.OpcUa.Client.Models;

public class OpcUaService : IOpcUaService
{
    private readonly IConnectionService _connection;
    private readonly INodeReadWriteService _readWrite;
    private readonly INodeBrowseService _browser;
    private readonly IDiscoveryService _discovery;

    public OpcUaService(
        IConnectionService connection,
        INodeReadWriteService readWrite,
        INodeBrowseService browser,
        IDiscoveryService discovery)
    {
        _connection = connection;
        _readWrite = readWrite;
        _browser = browser;
        _discovery = discovery;
    }

    public Task ConnectAsync(string endpointUrl) => _connection.ConnectAsync(endpointUrl);
    public Task DisconnectAsync() => _connection.DisconnectAsync();
    public Task<ConnectionResult<NodeReadResult>> ReadNodeAsync(string nodeId) => _readWrite.ReadNodeAsync(nodeId);
    public Task WriteNodeAsync(string nodeId, object value, CancellationToken ct) => _readWrite.WriteNodeAsync(nodeId, value, ct);
    public List<string> Browse(string nodeId) => _browser.Browse(nodeId);
    public Task<List<string>> FindServersOnLocalNetworkAsync() => _discovery.FindServersOnLocalNetworkAsync();
}

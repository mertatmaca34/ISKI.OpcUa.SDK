﻿using ISKI.OpcUa.Client.Models;

namespace ISKI.OpcUa.Client.Interfaces;

public interface IOpcUaService
{
    Task ConnectAsync(string endpointUrl);
    Task DisconnectAsync();
    Task<ConnectionResult<NodeReadResult>> ReadNodeAsync(string nodeId);
    Task WriteNodeAsync(string nodeId, object value, CancellationToken cancellationToken);
    List<NodeBrowseResult> Browse(string nodeId);
    NodeTreeResult BrowseTree(string nodeId);
    Task<List<string>> FindServersOnLocalNetworkAsync();
    Task<List<string>> FindServersOnLocalNetworkAsync(string networkPrefix = "192.168.1", int port = 4840);
}
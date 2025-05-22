using Opc.Ua;
using Opc.Ua.Client;
using Microsoft.Extensions.Logging;
using ISKI.OpcUa.Client.Exceptions;
using ISKI.OpcUa.Client.Interfaces;
using ISKI.OpcUa.Client.Models;

namespace ISKI.OpcUa.Client.Services;

public class NodeReadWriteService : INodeReadWriteService
{
    private readonly ILogger<NodeReadWriteService> _logger;
    private readonly IConnectionService _connection;

    public NodeReadWriteService(ILogger<NodeReadWriteService> logger, IConnectionService connection)
    {
        _logger = logger;
        _connection = connection;
    }

    public async Task<ConnectionResult<NodeReadResult>> ReadNodeAsync(string nodeId)
    {
        var session = _connection.Session;

        if (session == null || !session.Connected)
        {
            var msg = "OPC UA oturumu bağlı değil.";
            _logger.LogWarning("ReadNode - {msg}", msg);
            return new ConnectionResult<NodeReadResult>
            {
                ServerStatus = "Disconnected",
                Message = msg,
                Data = new NodeReadResult
                {
                    NodeId = nodeId,
                    NodeStatus = "NoSession",
                    Value = null
                }
            };
        }

        try
        {
            var node = new NodeId(nodeId);
            var value = await session.ReadValueAsync(node);

            var result = new NodeReadResult
            {
                NodeId = nodeId,
                Value = value.Value,
                NodeStatus = value.StatusCode.ToString()
            };

            var response = new ConnectionResult<NodeReadResult>
            {
                ServerStatus = value.StatusCode.ToString(),
                Message = StatusCode.IsGood(value.StatusCode)
                    ? "Okuma başarılı."
                    : $"OPC veri durumu geçersiz: {value.StatusCode}",
                Data = result,
                Timestamp = value.ServerTimestamp
            };

            if (response.Success)
                _logger.LogInformation("ReadNode başarılı: {nodeId} = {value}", nodeId, result.Value);
            else
                _logger.LogWarning("ReadNode başarısız: {nodeId}, Status: {status}", nodeId, result.NodeStatus);

            return response;
        }
        catch (Exception ex)
        {
            var msg = $"ReadNode exception: {ex.Message}";
            _logger.LogError(ex, msg);
            return new ConnectionResult<NodeReadResult>
            {
                ServerStatus = "Exception",
                Message = msg,
                Data = new NodeReadResult
                {
                    NodeId = nodeId,
                    NodeStatus = "Exception",
                    Value = null
                }
            };
        }
    }

    public async Task<ConnectionResult<object>> WriteNodeAsync(string nodeId, object value, CancellationToken cancellationToken)
    {
        var session = _connection.Session;

        if (session == null)
        {
            var msg = "OPC UA oturumu bağlı değil.";
            _logger.LogWarning("WriteNode - {msg}", msg);
            return new ConnectionResult<object>
            {
                ServerStatus = "Disconnected",
                Message = msg,
                Data = null
            };
        }

        try
        {
            var writeValue = new WriteValue
            {
                NodeId = new NodeId(nodeId),
                AttributeId = Attributes.Value,
                Value = new DataValue(new Variant(value))
            };

            var result = await session.WriteAsync(null, new WriteValueCollection { writeValue }, cancellationToken);
            var status = result.Results.FirstOrDefault();

            var response = new ConnectionResult<object>
            {
                ServerStatus = status.ToString(),
                Message = StatusCode.IsGood(status)
                    ? "Yazma işlemi başarılı."
                    : $"Yazma başarısız. OPC Status: {status}",
                Data = null
            };

            if (response.Success)
                _logger.LogInformation("WriteNode başarılı: {nodeId} = {value}", nodeId, value);
            else
                _logger.LogWarning("WriteNode başarısız: {nodeId}, Status: {status}", nodeId, status);

            return response;
        }
        catch (Exception ex)
        {
            var msg = $"WriteNode exception: {ex.Message}";
            _logger.LogError(ex, msg);
            return new ConnectionResult<object>
            {
                ServerStatus = "Exception",
                Message = msg,
                Data = null
            };
        }
    }
}

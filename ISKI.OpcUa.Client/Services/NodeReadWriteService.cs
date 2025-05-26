using Opc.Ua;
using Opc.Ua.Client;
using Microsoft.Extensions.Logging;
using ISKI.OpcUa.Client.Exceptions;
using ISKI.OpcUa.Client.Interfaces;
using ISKI.OpcUa.Client.Models;

namespace ISKI.OpcUa.Client.Services;

public class NodeReadWriteService(ILogger<NodeReadWriteService> logger, IConnectionService connection) : INodeReadWriteService
{
    public async Task<ConnectionResult<NodeReadResult>> ReadNodeAsync(string nodeId)
    {
        var session = connection.Session;

        if (session == null || !session.Connected)
        {
            var msg = "OPC UA oturumu bağlı değil.";
            logger.LogWarning("ReadNode - {Message}", msg);
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
                logger.LogInformation("ReadNode başarılı: {NodeId} = {Value}", nodeId, result.Value);
            else
                logger.LogWarning("ReadNode başarısız: {NodeId}, Status: {NodeStatus}", nodeId, result.NodeStatus);

            return response;
        }
        catch (Exception ex)
        {
            var msg = $"ReadNode exception: {ex.Message}";
            logger.LogError(ex, "ReadNode exception: {Message}", ex.Message);
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
        var session = connection.Session;

        if (session == null)
        {
            var msg = "OPC UA oturumu bağlı değil.";
            logger.LogWarning("WriteNode - {Message}", msg);
            return new ConnectionResult<object>
            {
                ServerStatus = "Disconnected",
                Message = msg,
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

            var result = await session.WriteAsync(null, [writeValue], cancellationToken);
            var status = result.Results.FirstOrDefault();

            var response = new ConnectionResult<object>
            {
                ServerStatus = status.ToString(),
                Message = StatusCode.IsGood(status)
                    ? "Yazma işlemi başarılı."
                    : $"Yazma başarısız. OPC Status: {status}",
            };

            if (response.Success)
                logger.LogInformation("WriteNode başarılı: {NodeId} = {Value}", nodeId, value);
            else
                logger.LogWarning("WriteNode başarısız: {NodeId}, Status: {Status}", nodeId, status);

            return response;
        }
        catch (Exception ex)
        {
            var msg = $"WriteNode exception: {ex.Message}";
            logger.LogError(ex, "WriteNode exception: {Message}", ex.Message);
            return new ConnectionResult<object>
            {
                ServerStatus = "Exception",
                Message = msg,
            };
        }
    }
}

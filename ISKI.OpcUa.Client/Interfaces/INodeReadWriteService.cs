using ISKI.OpcUa.Client.Models;

namespace ISKI.OpcUa.Client.Interfaces;

public interface INodeReadWriteService
{
    Task<ConnectionResult<NodeReadResult>> ReadNodeAsync(string nodeId);
    Task<ConnectionResult<object>> WriteNodeAsync(string nodeId, object value, CancellationToken cancellationToken);
}

using Opc.Ua.Client;

namespace ISKI.OpcUa.Client.Interfaces;

public interface IConnectionService
{
    Task ConnectAsync(string endpointUrl);
    Task DisconnectAsync();
    Session? Session { get; }
}

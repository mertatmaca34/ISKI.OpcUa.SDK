namespace ISKI.OpcUa.Client.Interfaces;

public interface IDiscoveryService
{
    Task<List<string>> FindServersOnLocalNetworkAsync(string networkPrefix = "192.168.1", int port = 4840);
}

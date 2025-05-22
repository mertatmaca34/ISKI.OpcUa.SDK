namespace ISKI.OpcUa.Client.Interfaces;

public interface IDiscoveryService
{
    Task<List<string>> FindServersOnLocalNetworkAsync();
}

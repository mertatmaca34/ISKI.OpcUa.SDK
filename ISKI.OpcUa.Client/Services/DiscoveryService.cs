using Opc.Ua;
using Opc.Ua.Client;
using Microsoft.Extensions.Logging;
using ISKI.OpcUa.Client.Interfaces;

namespace ISKI.OpcUa.Client.Services;

public class DiscoveryService : IDiscoveryService
{
    private readonly ILogger<DiscoveryService> _logger;

    public DiscoveryService(ILogger<DiscoveryService> logger)
    {
        _logger = logger;
    }

    public async Task<List<string>> FindServersOnLocalNetworkAsync()
    {
        var foundServers = new List<string>();
        var port = 4840;
        var tasks = new List<Task>();

        _logger.LogInformation("Yerel ağda OPC sunucuları aranıyor...");

        for (int i = 1; i <= 254; i++)
        {
            string ip = $"192.168.1.{i}";
            string endpoint = $"opc.tcp://{ip}:{port}";

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    using var client = DiscoveryClient.Create(new Uri(endpoint));
                    var servers = await client.FindServersAsync(null);

                    lock (foundServers)
                    {
                        foreach (var server in servers)
                        {
                            string info = $"{server.ApplicationName.Text} - {endpoint}";
                            foundServers.Add(info);
                            _logger.LogInformation("Sunucu bulundu: {info}", info);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug("IP {ip} üzerinde sunucu bulunamadı. Hata: {msg}", ip, ex.Message);
                }
            }));
        }

        await Task.WhenAll(tasks);

        _logger.LogInformation("Yerel ağ taraması tamamlandı. Toplam sunucu: {count}", foundServers.Count);
        return foundServers;
    }
}

using Opc.Ua;
using Opc.Ua.Client;
using Microsoft.Extensions.Logging;
using ISKI.OpcUa.Client.Interfaces;

namespace ISKI.OpcUa.Client.Services;

public class DiscoveryService(ILogger<DiscoveryService> logger) : IDiscoveryService
{
    public async Task<List<string>> FindServersOnLocalNetworkAsync(string networkPrefix = "192.168.1", int port = 4840)
    {
        var foundServers = new List<string>();
        var tasks = new List<Task>();

        logger.LogInformation("Yerel ağ {networkPrefix}.x üzerinde OPC sunucuları aranıyor (port {port})...", networkPrefix, port);

        for (int i = 1; i <= 254; i++)
        {
            string ip = $"{networkPrefix}.{i}";
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
                            logger.LogInformation("Sunucu bulundu: {info}", info);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogDebug("IP {ip} üzerinde sunucu bulunamadı. Hata: {msg}", ip, ex.Message);
                }
            }));
        }

        await Task.WhenAll(tasks);

        logger.LogInformation("Yerel ağ taraması tamamlandı. Toplam sunucu: {count}", foundServers.Count);
        return foundServers;
    }
}

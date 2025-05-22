using ISKI.OpcUa.Client.Interfaces;
using ISKI.OpcUa.Client.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ISKI.OpcUa.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIskiOpcUaClient(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionService, ConnectionService>();
        services.AddSingleton<INodeReadWriteService, NodeReadWriteService>();
        services.AddSingleton<INodeBrowseService, NodeBrowseService>();
        services.AddSingleton<IDiscoveryService, DiscoveryService>();
        services.AddSingleton<IOpcUaService, OpcUaService>();

        return services;
    }
}

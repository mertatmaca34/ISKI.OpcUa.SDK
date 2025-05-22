using ISKI.OpcUa.Client.Interfaces;
using ISKI.OpcUa.Client.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ISKI.OpcUa.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIskiOpcUaClient(this IServiceCollection services)
    {
        services.AddScoped<IConnectionService, ConnectionService>();
        services.AddScoped<INodeReadWriteService, NodeReadWriteService>();
        services.AddScoped<INodeBrowseService, NodeBrowseService>();
        services.AddScoped<IDiscoveryService, DiscoveryService>();
        services.AddScoped<IOpcUaService, OpcUaService>();

        return services;
    }
}

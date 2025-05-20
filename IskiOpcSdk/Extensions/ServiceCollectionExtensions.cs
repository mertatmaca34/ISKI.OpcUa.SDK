using IskiOpcSdk.Interfaces;
using IskiOpcSdk.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskiOpcSdk.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpcUaClient(this IServiceCollection services)
    {
        services.AddSingleton<IOpcUaService, OpcUaService>();
        return services;
    }
}
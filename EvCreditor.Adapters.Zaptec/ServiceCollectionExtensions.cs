using EvCreditor.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EvCreditor.Adapters.Zaptec;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddZaptecSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ZaptecOptions>(configuration.GetSection(ZaptecOptions.SectionName));
        return services;
    }

    public static IServiceCollection AddZaptecServices(this IServiceCollection services)
    {
        services.AddHttpClient<IChargingReportService, ZapTecReportService>();
        return services;
    }
}
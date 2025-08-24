using EvCreditor.Abstractions.Services;
using EvCreditor.Adapters.Billit.Models;
using EvCreditor.Adapters.Billit.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EvCreditor.Adapters.Billit;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBillit(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BillitOptions>(configuration.GetSection("Billit"));
        services.AddHttpClient<ICreditNoteService, BillitService>();
        return services;
    }
}

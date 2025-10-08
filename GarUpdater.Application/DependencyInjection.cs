using GarUpdater.Core.Interfaces;
using GarUpdater.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GarUpdater.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddGarUpdater(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("GarUpdaterClient")
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();

                handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12
                                     | System.Security.Authentication.SslProtocols.Tls13;

                return handler;
            })
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("GarUpdater / 1.0");
                client.Timeout = TimeSpan.FromMinutes(5);
            });

        services.AddSingleton<IGarDownloader, GarDownloader>();
        services.AddSingleton<IGarExtractor, GarExtractor>();
        services.AddSingleton<IGarXmlParser, GarXmlParser>();
        services.AddSingleton<ICsvExporter, CsvExporter>();
        services.AddSingleton<GarUpdateOrchestrator>();

        return services;
    }
}

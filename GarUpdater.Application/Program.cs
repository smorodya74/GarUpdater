using GarUpdater.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddGarUpdater(ctx.Configuration);
        services.AddLogging(lb => lb.AddConsole());
        services.AddSingleton<GarUpdateOrchestrator>();
    })
    .Build();

using var scope = host.Services.CreateScope();
var orchestrator = scope.ServiceProvider.GetRequiredService<GarUpdateOrchestrator>();

try
{
    await orchestrator.RunAsync();
}
finally
{
    await host.StopAsync();
}

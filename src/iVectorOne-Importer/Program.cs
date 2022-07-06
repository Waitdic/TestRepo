using Intuitive.Modules;
using iVectorOne.Importer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

await new HostBuilder()
    .UseDiscoveredModules()
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<OwnStockPropertyImportService>();
    })
    .ConfigureLogging((ctx, lb) =>
    {
        lb.AddConfiguration(ctx.Configuration);
        lb.AddConsole();
    })
    .RunConsoleAsync();
using Intuitive.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

await new HostBuilder()
    .UseDiscoveredModules()
    .ConfigureLogging((ctx, lb) =>
    {
        lb.AddConfiguration(ctx.Configuration);
        lb.AddConsole();
    })
    .RunConsoleAsync();
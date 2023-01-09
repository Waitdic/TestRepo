using Intuitive.Modules;
using iVectorOne.Content.Api.Endpoints.V1;
using iVectorOne.Content.Api.Endpoints.V2;
using iVectorOne.Web.Infrastructure.V2;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    ThreadPool.SetMinThreads(200, 200);

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.SetupLogging();

    // Add services to the container.
    builder.Services.RegisterWebServices(builder.Configuration);
    builder.Host.UseDiscoveredModules();

    var app = builder.Build();

    //Load application specific endpoints
    app.MapEndpointsV1();
    app.MapEndpoints();

    // todo - integrate with health checks
    _ = app.MapGet("/healthcheck", () => "Hello World!").AllowAnonymous();

    app.BuildAndRun();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
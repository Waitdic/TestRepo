using Intuitive.Modules;
using iVectorOne.Search.Api.Endpoints.V1;
using iVectorOne.Search.Api.Endpoints.V2;
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
    app.MapTransferEndpoints();

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
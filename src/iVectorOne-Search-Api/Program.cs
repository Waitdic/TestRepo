using Intuitive.Modules;
using iVectorOne.Search.Api.Endpoints.V1;
using iVectorOne.Search.Api.Endpoints.V2;
using iVectorOne.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Diagnostics;
using System.Net;

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

    app.UseExceptionHandler("/error");

    app.MapGet("/error", () =>
    {
        var problemDetails = new ProblemDetails
        {
            Title = "An unexpected error occurred processing your request.",
        };

        problemDetails.Extensions.Add(new KeyValuePair<string, object?>("TraceId", Activity.Current?.Id));

        return Results.Problem(problemDetails);
    })
    .ExcludeFromDescription();

    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 |
            SecurityProtocolType.Tls;

    app.UseSerilogRequestLogging();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseHttpsRedirection();

    app.Run();
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
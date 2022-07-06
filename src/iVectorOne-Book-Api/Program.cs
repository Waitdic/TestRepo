using System.Diagnostics;
using Intuitive.Modules;
using iVectorOne.Book.Api.Endpoints.V2;
using iVectorOne.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    ThreadPool.SetMinThreads(200, 200);

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console()
        .Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("/error")))
        .ReadFrom.Configuration(ctx.Configuration));

    // Add services to the container.
    builder.Services.RegisterWebServices(builder.Configuration);
    builder.Host.UseDiscoveredModules();

    var app = builder.Build();

    //Load application specific endpoints
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
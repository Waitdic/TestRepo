using System.Diagnostics;
using Intuitive.Modules;
using iVectorOne.Content.Api.Endpoints.V1;
using iVectorOne.Content.Api.Endpoints.V2;
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

    //app.UseExceptionHandler(e =>
    //{
    //    e.Run(context =>
    //    {
    //        string title = "An unexpected error occurred processing your request.";
    //        int statusCode = 500;

    //        var exception = context.Features.Get<IExceptionHandlerPathFeature>();

    //        if (exception?.Error is ValidationException ex)
    //        {
    //            title = string.Join(Environment.NewLine, ex.Errors.Select(e=> e.ErrorMessage));
    //            statusCode = 403;
    //        }

    //        var problemDetails = new ProblemDetails
    //        {
    //            Title = title,
    //            Status = statusCode,
    //        };

    //        problemDetails.Extensions.Add(new KeyValuePair<string, object?>("TraceId", Activity.Current?.Id));

    //        return Task.FromResult(Results.Problem(problemDetails));
    //    });
    //});

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
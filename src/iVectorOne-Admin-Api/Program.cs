using iVectorOne_Admin_Api.Config.Models;
using iVectorOne_Admin_Api.Config.Requests;
using iVectorOne_Admin_Api.Config.Responses;
using iVectorOne_Admin_Api.Features;
using iVectorOne_Admin_Api.Infrastructure;
using iVectorOne_Admin_Api.Security;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

var AssemblyInfo = Assembly.GetExecutingAssembly().FullName;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information($"Starting up {AssemblyInfo}");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment}.json", optional: true)
        .AddEnvironmentVariables();

    builder
        .AddApplicationServices()
        .AddIntuitiveLogging()
        .AddIntuitiveAuditing()
        .AddIntuitiveCors()
        .AddIntuitiveAuthentication();

    builder.Services.AddHealthChecks()
        .AddSqlServer(builder.Configuration.GetConnectionString("ConfigConnection"));

    var app = builder.Build();

    app
        .UseIntuitiveLogging()
        .UseIntuitiveAuditing()
        .UseHttpsRedirection()
        .UseIntuitiveMiddleware()
        .UseAuthentication()
        .UseAuthorization()
        .UseIntuitiveCors()
        .UseExceptionHandler("/Error");

    app.MapHealthChecks("/healthcheck").AllowAnonymous();

    app.AddFeatures();

    //app.MapGet("v1/tenants/{tenantid}/accounts/{accountid}/suppliers/{supplierid}", async (IMediator mediator, HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey, int tenantid, int accountid, int supplierid) =>
    //{
    //    if (httpContext.User.Identity is not TenantIdentity identity)
    //    {
    //        return Results.Challenge();
    //    }

    //    SupplierResponse response = null!;

    //    try
    //    {
    //        var request = new SupplierRequest(tenantid) { AccountId = accountid, SupplierId = supplierid };
    //        response = await mediator.Send(request);
    //    }
    //    catch (Exception e)
    //    {
    //        return Results.Problem(e.ToString());
    //    }

    //    return Results.Ok(response);
    //}).RequireAuthorization();

    //app.MapPut(
    //        "v1/tenants/{tenantid}/accounts/{accountid}/suppliers/{supplierid}/accountsupplierattributes",
    //        async (IMediator mediator,
    //        HttpContext httpContext,
    //        [FromHeader(Name = "TenantKey")] Guid tenantKey,
    //        int tenantid,
    //        int accountid,
    //        int supplierid,
    //        [FromBody] SupplierAttributeUpdateDTO updateRequest) =>
    //    {
    //        if (httpContext.User.Identity is not TenantIdentity identity)
    //        {
    //            return Results.Challenge();
    //        }

    //        SupplierAttributeUpdateResponse response = null!;

    //        try
    //        {
    //            var request = new SupplierAttributeUpdateRequest(tenantid)
    //            {
    //                AccountId = accountid,
    //                SupplierId = supplierid,
    //                Attributes = updateRequest
    //            };
    //            response = await mediator.Send(request);
    //        }
    //        catch (ValidationException ex)
    //        {
    //            //return Results.ValidationProblem(new Dictionary<string, string[]> { { "Validation Error", ex.Errors.Select(x => x.ErrorMessage).ToArray() } });
    //        }
    //        return Results.Ok(response);
    //    }).RequireAuthorization();

    //app.MapPut("v1/tenants/{tenantid}/accounts/{accountid}/suppliers/{supplierid}",
    //    async (IMediator mediator,
    //            HttpContext httpContext,
    //            [FromHeader(Name = "TenantKey")] Guid tenantKey,
    //            int tenantid,
    //            int accountid,
    //            int supplierid,
    //            [FromBody] AccountSupplierUpdateDTO updateRequest) =>
    //    {
    //        if (httpContext.User.Identity is not TenantIdentity identity)
    //        {
    //            return Results.Challenge();
    //        }

    //        AccountSupplierUpdateResponse response = null!;

    //        try
    //        {
    //            var request = new AccountSupplierUpdateRequest(tenantid)
    //            {
    //                TenantId = tenantid,
    //                AccountId = accountid,
    //                SupplierId = supplierid,
    //                Enabled = updateRequest.Enabled
    //            };
    //            response = await mediator.Send(request);
    //        }
    //        catch (ValidationException ex)
    //        {
    //            //return Results.ValidationProblem(new Dictionary<string, string[]> { { "Validation Error", ex.Errors.Select(x => x.ErrorMessage).ToArray() } });
    //        }
    //        return Results.Ok(response);
    //    }).RequireAuthorization();

    //app.MapGet("v1/suppliers", async (IMediator mediator, HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey) =>
    //{
    //    if (httpContext.User.Identity is not TenantIdentity identity)
    //    {
    //        return Results.Challenge();
    //    }

    //    SupplierListResponse response = null!;

    //    try
    //    {
    //        var request = new SupplierListRequest();
    //        response = await mediator.Send(request);
    //    }
    //    catch (Exception e)
    //    {
    //        return Results.Problem(e.ToString());
    //    }

    //    return Results.Ok(response);
    //}).RequireAuthorization();

    //app.MapGet("v1/suppliers/{supplierid}", async (IMediator mediator, HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey, int supplierid) =>
    //{
    //    if (httpContext.User.Identity is not TenantIdentity identity)
    //    {
    //        return Results.Challenge();
    //    }

    //    SupplierAttributeResponse response = null!;

    //    try
    //    {
    //        var request = new SupplierAttributeRequest() { SupplierID = supplierid };
    //        response = await mediator.Send(request);
    //    }
    //    catch (Exception e)
    //    {
    //        return Results.Problem(e.ToString());
    //    }

    //    return Results.Ok(response);
    //}).RequireAuthorization();

    //app.MapPost("v1/tenants/{tenantid}/accounts/{accountid}/suppliers/{supplierid}",
    //    async (IMediator mediator,
    //        HttpContext httpContext,
    //        [FromHeader(Name = "TenantKey")] Guid tenantKey,
    //        int tenantid,
    //        int accountid,
    //        int supplierid,
    //        [FromBody] AccountSupplierCreateDTO createRequest) =>
    //    {
    //        if (httpContext.User.Identity is not TenantIdentity identity)
    //        {
    //            return Results.Challenge();
    //        }
    //        AccountSupplierCreateResponse response = null!;

    //        try
    //        {
    //            var request = new AccountSupplierCreateRequest(tenantid)
    //            {
    //                TenantId = tenantid,
    //                AccountId = accountid,
    //                SupplierId = supplierid,
    //                SupplierAttributes = createRequest
    //            };
    //            response = await mediator.Send(request);
    //        }
    //        catch (ValidationException ex)
    //        {
    //            //return Results.ValidationProblem(new Dictionary<string, string[]> { { "Validation Error", ex.Errors.Select(x => x.ErrorMessage).ToArray() } });
    //        }
    //        return Results.Ok(response);
    //    }).RequireAuthorization();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, $"Unhandled exception in {AssemblyInfo}");
}
finally
{
    Log.Information($"Shut down complete {AssemblyInfo}");
    Log.CloseAndFlush();
}
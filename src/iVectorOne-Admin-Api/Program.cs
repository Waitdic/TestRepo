using FluentValidation;
using iVectorOne_Admin_Api.Config.Models;
using iVectorOne_Admin_Api.Config.Requests;
using iVectorOne_Admin_Api.Config.Responses;
using iVectorOne_Admin_Api.Security;
using iVectorOne_Admin_Api.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.RegisterServices();

var app = builder.Build();
app.ConfigureApp();

app.MapGet("v1/users/{key}", async (IMediator mediator, string key) =>
{
    var request = new UserRequest(key);
    var user = await mediator.Send(request);

    return Results.Ok(user);
});

app.MapGet("v1/tenants/{tenantid}/subscriptions", async (IMediator mediator, HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey, int tenantid) =>
{
    if (httpContext.User.Identity is not TenantIdentity identity)
    {
        return Results.Challenge();
    }

    TenantResponse response = default;

    try
    {
        var request = new TenantRequest(tenantid);
        response = await mediator.Send(request);
    }
    catch (Exception e)
    {
        return Results.Problem(e.ToString());
    }

    return Results.Ok(response);
}).RequireAuthorization();

app.MapGet("v1/tenants/{tenantid}/subscriptions/{subscriptionid}", async (IMediator mediator, HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey, int tenantid, int subscriptionid) =>
{
    if (httpContext.User.Identity is not TenantIdentity identity)
    {
        return Results.Challenge();
    }

    SubscriptionResponse response = default;

    try
    {
        var request = new SubscriptionRequest(tenantid) { SubscriptionId = subscriptionid };
        response = await mediator.Send(request);
    }
    catch (Exception e)
    {
        return Results.Problem(e.ToString());
    }

    return Results.Ok(response);
}).RequireAuthorization();

app.MapGet("v1/tenants/{tenantid}/subscriptions/{subscriptionid}/suppliers", async (IMediator mediator, HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey, int tenantid, int subscriptionid) =>
{
    if (httpContext.User.Identity is not TenantIdentity identity)
    {
        return Results.Challenge();
    }

    SupplierSubscriptionResponse response = default;

    try
    {
        var request = new SupplierSubscriptionRequest(tenantid) { SubscriptionId = subscriptionid };
        response = await mediator.Send(request);
    }
    catch (Exception e)
    {
        return Results.Problem(e.ToString());
    }

    return Results.Ok(response);
}).RequireAuthorization();

app.MapGet("v1/tenants/{tenantid}/subscriptions/{subscriptionid}/suppliers/{supplierid}", async (IMediator mediator, HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey, int tenantid, int subscriptionid, int supplierid) =>
{
    if (httpContext.User.Identity is not TenantIdentity identity)
    {
        return Results.Challenge();
    }

    SupplierResponse response = default;

    try
    {
        var request = new SupplierRequest(tenantid) { SubscriptionId = subscriptionid, SupplierId = supplierid };
        response = await mediator.Send(request);
    }
    catch (Exception e)
    {
        return Results.Problem(e.ToString());
    }

    return Results.Ok(response);
}).RequireAuthorization();

app.MapPut(
        "v1/tenants/{tenantid}/subscriptions/{subscriptionid}/suppliers/{supplierid}/suppliersubscriptionattributes",
        async (IMediator mediator,
        HttpContext httpContext,
        [FromHeader(Name = "TenantKey")] Guid tenantKey,
        int tenantid,
        int subscriptionid,
        int supplierid,
        [FromBody] SupplierAttributeUpdateDTO updateRequest) =>
    {
        if (httpContext.User.Identity is not TenantIdentity identity)
        {
            return Results.Challenge();
        }

        SupplierAttributeUpdateResponse response = default;

        try
        {
            var request = new SupplierAttributeUpdateRequest(tenantid)
            {
                SubscriptionId = subscriptionid,
                SupplierId = supplierid,
                Attributes = updateRequest
            };
            response = await mediator.Send(request);
        }
        catch (ValidationException ex)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]> { { "Validation Error", ex.Errors.Select(x => x.ErrorMessage).ToArray() } });
        }
        return Results.Ok(response);
    }).RequireAuthorization();

app.MapPut("v1/tenants/{tenantid}/subscriptions/{subscriptionid}/suppliers/{supplierid}",
    async (IMediator mediator,
            HttpContext httpContext,
            [FromHeader(Name = "TenantKey")] Guid tenantKey,
            int tenantid,
            int subscriptionid,
            int supplierid,
            [FromBody] SupplierSubscriptionUpdateDTO updateRequest) =>
    {
        if (httpContext.User.Identity is not TenantIdentity identity)
        {
            return Results.Challenge();
        }

        SupplierSubscriptionUpdateResponse response = default;

        try
        {
            var request = new SupplierSubscriptionUpdateRequest(tenantid)
            {
                TenantId = tenantid,
                SubscriptionId = subscriptionid,
                SupplierId = supplierid,
                Enabled = updateRequest.Enabled
            };
            response = await mediator.Send(request);
        }
        catch (ValidationException ex)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]> { { "Validation Error", ex.Errors.Select(x => x.ErrorMessage).ToArray() } });
        }
        return Results.Ok(response);
    }).RequireAuthorization();

app.ConfigureSwagger();

app.Run();
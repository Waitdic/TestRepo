using iVectorOne_Admin_Api.Config.Requests;
using iVectorOne_Admin_Api.Security;
using iVectorOne_Admin_Api.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.RegisterServices();

var app = builder.Build();
app.ConfigureApp();

app.MapGet("/user/{key}", async (IMediator mediator, string key) =>
{
    var request = new UserRequest(key);
    var user = await mediator.Send(request);

    return user;
});

app.MapGet("/tenants/subscriptions", async (IMediator mediator, HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey) =>
{
    if (httpContext.User.Identity is not TenantIdentity identity)
    {
        return Results.Challenge();
    }

    TenantResponse response = default;

    try
    {
        var request = new TenantRequest(identity.Tenant.TenantId);
        response = await mediator.Send(request);
    }
    catch (Exception e)
    {
        return Results.Problem(e.ToString());
    }

    return Results.Ok(response);
}).RequireAuthorization();

app.MapGet("/tenants/subscriptions/{subscriptionid}", async (IMediator mediator, HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey, int subscriptionid) =>
{
    if (httpContext.User.Identity is not TenantIdentity identity)
    {
        return Results.Challenge();
    }

    SubscriptionResponse response = default;

    try
    {
        var request = new SubscriptionRequest(identity.Tenant.TenantId) { SubscriptionId = subscriptionid };
        response = await mediator.Send(request);
    }
    catch (Exception e)
    {
        return Results.Problem(e.ToString());
    }

    return Results.Ok(response);
}).RequireAuthorization();

app.MapGet("/tenants/subscriptions/{subscriptionid}/suppliers", async (IMediator mediator, HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey, int subscriptionid) =>
{
    if (httpContext.User.Identity is not TenantIdentity identity)
    {
        return Results.Challenge();
    }

    SupplierSubscriptionResponse response = default;

    try
    {
        var request = new SupplierSubscriptionRequest(identity.Tenant.TenantId) { SubscriptionId = subscriptionid };
        response = await mediator.Send(request);
    }
    catch (Exception e)
    {
        return Results.Problem(e.ToString());
    }

    return Results.Ok(response);
}).RequireAuthorization();

app.ConfigureSwagger();

app.Run();
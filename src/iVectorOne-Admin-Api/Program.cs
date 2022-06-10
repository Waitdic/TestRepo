using AutoMapper;
using iVectorOne_Admin_Api.Config.Context;
using iVectorOne_Admin_Api.Config.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ConfigContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConfigConnection")));
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseHttpsRedirection();

app.MapGet("/tenants/{tenantid}/subscriptions", (ConfigContext context, IMapper mapper, int tenantid) =>
{
    var subscriptions = context.Subscriptions.Where(t => t.TenantId == tenantid).ToList();
    List<SubscriptionDTO> subList = mapper.Map<List<SubscriptionDTO>>(subscriptions);

    return subList;
});

app.MapGet("/tenants/{tenantid}/subscriptions/{subscriptionid}", (ConfigContext context, IMapper mapper, int tenantid, int subscriptionid) =>
{
    var subscription = context.Subscriptions.Where(s=>s.SubscriptionId == subscriptionid && s.TenantId == tenantid).FirstOrDefault();
    SubscriptionDTO sub = mapper.Map<SubscriptionDTO>(subscription);

    return sub;
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Run();
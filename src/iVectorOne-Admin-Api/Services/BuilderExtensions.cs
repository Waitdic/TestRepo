using iVectorOne_Admin_Api.Config.Context;
using iVectorOne_Admin_Api.Security;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace iVectorOne_Admin_Api.Services
{
    public static class BuilderExtensions
    {
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<ConfigContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConfigConnection")));
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors();
            builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
            builder.Services.AddScoped<ITenantService, TenantService>();
            builder.Services.AddAuthentication("TenantAuthorisation")
                .AddScheme<AuthenticationSchemeOptions, TenantAuthorisationHandler>("TenantAuthorisation", null);
            builder.Services.AddAuthorization();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return builder;
        }
    }
}

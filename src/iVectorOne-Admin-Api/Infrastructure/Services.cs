using FluentValidation;
using Intuitive.Helpers;
using Intuitive.Helpers.Security;
using iVectorOne_Admin_Api.Config.Validation;
using iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Test;
using iVectorOne_Admin_Api.Security;
using System.Reflection;
using System.Security.Cryptography;

namespace iVectorOne_Admin_Api.Infrastructure
{
    public static class Services
    {
        public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddDbContext<AdminContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConfigConnection")));

            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

            builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            builder.Services.AddValidatorsFromAssemblyContaining<SupplierAttributeUpdateValidator>();

            builder.Services.AddHttpClient();
            //builder.Services.AddHttpClient<IRequestHandler<Request, Response>, Handler>(
            //    "default",
            //    client =>
            //    {
            //        client.BaseAddress = new Uri(builder.Configuration.GetSection("SearchService").Value);
            //    });

            builder.Services.AddScoped<ITenantService, TenantService>();

            builder.Services.AddHelperServices(builder.Configuration);
            builder.Services.AddTransient(c => c.GetRequiredService<ISecretKeeperFactory>().CreateSecretKeeper("FireyNebulaIsGod", EncryptionType.Aes, CipherMode.ECB));

            return builder;
        }
    }
}
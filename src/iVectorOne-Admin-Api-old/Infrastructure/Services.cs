namespace iVectorOne_Admin_Api.Infrastructure
{
    using System.Reflection;
    using System.Security.Cryptography;
    using FluentValidation;
    using Intuitive.Helpers;
    using Intuitive.Helpers.Security;
    using iVectorOne_Admin_Api.Config.Validation;
    using iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Test;
    using iVectorOne_Admin_Api.Security;
    using Microsoft.Extensions.DependencyInjection;

    public static class Services
    {
        public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<ConfigContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConfigConnection")));

            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
            builder.Services.AddValidatorsFromAssemblyContaining<SupplierAttributeUpdateValidator>();
            builder.Services.AddHelperServices(builder.Configuration);
            builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            builder.Services.AddHttpClient<IRequestHandler<Request, Response>, Handler>(
                "default",
                client =>
                {
                    client.BaseAddress = new Uri(builder.Configuration.GetSection("SearchService").Value);
                });

            builder.Services.AddScoped<ITenantService, TenantService>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddTransient(c => c.GetRequiredService<ISecretKeeperFactory>().CreateSecretKeeper("FireyNebulaIsGod", EncryptionType.Aes, CipherMode.ECB));

            return builder;
        }
    }
}
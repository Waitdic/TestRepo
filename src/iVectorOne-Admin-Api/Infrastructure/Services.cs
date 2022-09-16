namespace iVectorOne_Admin_Api.Infrastructure
{
    using FluentValidation;
    using Intuitive.Helpers;
    using iVectorOne_Admin_Api.Config.Validation;
    using iVectorOne_Admin_Api.Security;
    using System.Reflection;

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

            builder.Services.AddScoped<ITenantService, TenantService>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return builder;
        }
    }
}

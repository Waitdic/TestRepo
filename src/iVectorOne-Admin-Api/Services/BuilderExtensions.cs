//using iVectorOne_Admin_Api.Security;
//using MediatR;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.EntityFrameworkCore;
//using FluentValidation;
//using System.Reflection;
//using iVectorOne_Admin_Api.Config.Validation;
//using iVectorOne_Admin_Api.Data;

//namespace iVectorOne_Admin_Api.Services
//{
//    public static class BuilderExtensions
//    {
//        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
//        {
//            builder.Services.AddDbContext<ConfigContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConfigConnection")));
//            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
//            builder.Services.AddEndpointsApiExplorer();
//            builder.Services.AddSwaggerGen();
//            builder.Services.AddCors();
//            builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
//            builder.Services.AddScoped<ITenantService, TenantService>();
//            builder.Services.AddAuthentication("TenantAuthorisation")
//                .AddScheme<AuthenticationSchemeOptions, TenantAuthorisationHandler>("TenantAuthorisation", null);
//            builder.Services.AddAuthorization();
//            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//            builder.Services.AddValidatorsFromAssemblyContaining<SupplierAttributeUpdateValidator>();
//            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

//            return builder;
//        }
//    }
//}

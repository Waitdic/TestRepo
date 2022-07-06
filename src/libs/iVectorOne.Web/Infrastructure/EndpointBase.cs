namespace iVectorOne.Web.Infrastructure
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentValidation;
    using iVectorOne.Web.Infrastructure.Security;
    using iVectorOne.Web.Adaptors.Authentication;
    using MediatR;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.Extensions.DependencyInjection;
    using ThirdParty.SDK.V2;
    using Microsoft.Extensions.Configuration;

    public static class EndpointBase
    {
        public static IServiceCollection RegisterWebServices(this IServiceCollection services, ConfigurationManager config)
        {
            var userLoginMethod = config.GetValue<string>("UserLoginMethod");

            if (userLoginMethod == "SQL")
            {
                services.AddSingleton<IAuthenticationProvider, SqlAuthenticationProvider>();
            }
            else
            {
                services.AddSingleton<IAuthenticationProvider, FileAuthenticationProvider>();
            }

            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAuthorization();

            services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
            {
                options.SerializerOptions.PropertyNameCaseInsensitive = false;
                options.SerializerOptions.PropertyNamingPolicy = null;
                options.SerializerOptions.WriteIndented = true;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            return services;
        }

        public static async Task<IResult> ExecuteRequest<TRequest, TResponse>(HttpContext httpContext, IMediator mediator, TRequest request)
            where TRequest : RequestBase, IRequest<TResponse>
        {
            if (httpContext.User.Identity is not AuthenticationIdentity identity)
            {
                return Results.Challenge();
            }
            request.User = identity.User;
            TResponse? response = default;

            try
            {
                response = await mediator.Send(request);
            }
            catch (ValidationException ex)
            {
                return Results.ValidationProblem(ex.Errors.ToDictionary(e => e.ErrorCode, e => new string[] { e.ErrorMessage }));
            }

            return Results.Ok(response);
        }
    }
}
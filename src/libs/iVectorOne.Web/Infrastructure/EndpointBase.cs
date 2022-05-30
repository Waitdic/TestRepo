namespace iVectorOne.Web.Infrastructure
{
    using FluentValidation;
    using MediatR;
    using ThirdParty.SDK.V2;
    using IVectorOne.Web.Infrastructure.Security;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using IVectorOne.Web.Adaptors.Authentication;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.AspNetCore.Builder;

    public static class EndpointBase
    {
        public static IServiceCollection RegisterWebServices(this IServiceCollection services)
        {
            services.AddSingleton<IAuthenticationProvider, FileAuthenticationProvider>();

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
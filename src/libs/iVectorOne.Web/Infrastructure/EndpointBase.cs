namespace iVectorOne.Web.Infrastructure
{
    using System.Linq;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using FluentValidation;
    using iVectorOne.Web.Adaptors.Authentication;
    using iVectorOne.Web.Infrastructure.Security;
    using MediatR;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using ThirdParty.SDK.V2;

    public static class EndpointBase
    {
        public const string Version = "v2";
        public const string Domain = "property";

        public static IServiceCollection RegisterWebServices(this IServiceCollection services, ConfigurationManager config)
        {
            services.AddSingleton<IAuthenticationProvider, SqlAuthenticationProvider>();

            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAuthorization();

            services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
            {
                options.SerializerOptions.PropertyNameCaseInsensitive = false;
                options.SerializerOptions.PropertyNamingPolicy = null;
                options.SerializerOptions.WriteIndented = true;
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
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
            where TResponse : ResponseBase, new()
        {
            if (httpContext.User.Identity is not AuthenticationIdentity identity)
            {
                return Results.Challenge();
            }
            request.User = identity.User;
            var response = new TResponse();

            try
            {
                response = await mediator.Send(request);

                if (response is null)
                {
                    return Results.UnprocessableEntity();
                }
                else if (response.Warnings.Any())
                {
                    return Results.BadRequest(response.Warnings);
                }
            }
            catch (ValidationException ex)
            {
                return Results.ValidationProblem(ex.Errors.ToDictionary(e => e.ErrorCode, e => new string[] { e.ErrorMessage }));
            }

            return Results.Ok(response);
        }
    }
}
namespace iVectorOne.Web.Infrastructure.V2
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
    using iVectorOne.SDK.V2;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Filters;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;

    public static class EndpointBase
    {
        public const string Version = "v2";
        public const string Domain = "properties";

        public static IHostBuilder SetupLogging(this IHostBuilder host)
        {
            host.UseSerilog((ctx, lc) => lc
                .WriteTo.Console()
                .Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("/error")))
                .Filter.ByExcluding(Matching.WithProperty("RequestPath", "/healthcheck"))
                .ReadFrom.Configuration(ctx.Configuration));

            return host;
        }

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

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.ToString());
            });

            return services;
        }

        public static void BuildAndRun(this WebApplication app)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            app.UseExceptionHandler("/error");

            app.MapGet("/error", () =>
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "An unexpected error occurred processing your request.",
                };

                problemDetails.Extensions.Add(new KeyValuePair<string, object?>("TraceId", Activity.Current?.Id));

                return Results.Problem(problemDetails);
            })
            .ExcludeFromDescription();

            app.UseSerilogRequestLogging();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.Run();
        }

        public static async Task<IResult> ExecuteRequest<TRequest, TResponse>(HttpContext httpContext, IMediator mediator, TRequest request)
            where TRequest : RequestBase, IRequest<TResponse>
            where TResponse : ResponseBase, new()
        {
            if (httpContext.User.Identity is not AuthenticationIdentity identity)
            {
                return Results.Challenge();
            }
            request.Account = identity.Account;
            var response = new TResponse();

            try
            {
                response = await mediator.Send(request);

                if (response is null)
                {
                    return Results.UnprocessableEntity();
                }
                else if (response.Warnings?.Any() ?? false)
                {
                    return Results.BadRequest(response.Warnings);
                }
            }
            catch (ValidationException ex)
            {
                return Results.ValidationProblem(
                    ex.Errors
                        .GroupBy(e => e.ErrorCode)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()));
            }

            return Results.Ok(response);
        }
    }
}
using Audit.Core;
using Audit.WebApi;
using iVectorOne_Admin_Api.Infrastructure.Extensions;
using iVectorOne_Admin_Api.Security;
using Microsoft.AspNetCore.Authentication;
using Serilog;
using Serilog.Enrichers.Span;
using System.Diagnostics;

namespace iVectorOne_Admin_Api.Infrastructure
{
    public static class IntuitiveToolkit
    {
        #region Logging

        public static WebApplicationBuilder AddIntuitiveLogging(this WebApplicationBuilder builder)
        {
            var enableLogging = builder.Configuration.GetSection("Debug:EnableLogging").Get<bool>();

            if (!enableLogging)
                return builder;

            // Setup Logging
            builder.Host.UseSerilog((ctx, lc) => lc
                .Enrich.WithSpan()
                .ReadFrom.Configuration(ctx.Configuration));

            return builder;
        }

        public static WebApplication UseIntuitiveLogging(this WebApplication app)
        {

            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var enableLogging = app.Configuration.GetSection("Debug:EnableLogging").Get<bool>();

            if (!enableLogging)
                return app;

            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("Request-Id", httpContext.TraceIdentifier);
                };
            });

            return app;
        }

        #endregion

        #region Auditing

        public static WebApplicationBuilder AddIntuitiveAuditing(this WebApplicationBuilder builder)
        {
            var enableAuditing = builder.Configuration.GetSection("Debug:EnableAuditing").Get<bool>();

            if (!enableAuditing)
                return builder;

            // Setup Auditing
            var auditConnectionString = builder.Configuration.GetConnectionString("AuditDatabase");

            Configuration.Setup().UseSqlServer(config => config
                .ConnectionString(auditConnectionString)
                .Schema("dbo")
                .TableName("admin_audit")
                .IdColumnName("EventId")
                .JsonColumnName("JsonData")
                .LastUpdatedColumnName("LastUpdatedDate")
                .CustomColumn("EventType", ev => ev.EventType));

            return builder;
        }

        public static WebApplication UseIntuitiveAuditing(this WebApplication app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var enableLogging = app.Configuration.GetSection("Debug:EnableAuditing").Get<bool>();

            if (!enableLogging)
                return app;

            app.UseAuditMiddleware(_ => _
                .FilterByRequest(rq =>
                {
                    //Do not audit error page and get requests
                    var result = !rq.Path.Value.EndsWith("/error") &&
                                    rq.Method.ToLower() != "get" &&
                                    rq.Method.ToLower() != "options";

                    return result;
                })
                .WithEventType("HTTP:{verb}:{url}")
                .IncludeHeaders()
                .IncludeResponseHeaders()
                .IncludeRequestBody()
                .IncludeResponseBody());

            Configuration.AddCustomAction(ActionType.OnScopeCreated, scope =>
            {
                var activity = Activity.Current;
                scope.Event.CustomFields["RequestId"] = activity?.GetTraceId();
            });

            return app;
        }

        #endregion

        #region Middleware

        public static IApplicationBuilder UseIntuitiveMiddleware(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            // Return Request-Id headeer so caller can use in subsequent calls to correlate request together
            app.Use(async (context, next) =>
            {
                var activity = Activity.Current;
                context.Response.Headers.Add("Request-Id", activity?.GetTraceId());
                await next();
            });

            return app;
        }

        #endregion

        #region Cors

        public static WebApplicationBuilder AddIntuitiveCors(this WebApplicationBuilder builder)
        {
            var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
            var corsHeaders = builder.Configuration.GetSection("Cors:AllowedHeaders").Get<string[]>();
            var corsMethods = builder.Configuration.GetSection("Cors:AllowedMethods").Get<string[]>();

            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy(name: "CorsPolicy", builder =>
                {
                    builder.WithOrigins(corsOrigins)
                        .WithHeaders(corsHeaders)
                        .WithMethods(corsMethods);
                });
            });

            return builder;
        }

        public static IApplicationBuilder UseIntuitiveCors(this IApplicationBuilder app)
        {

            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.UseCors("CorsPolicy");

            return app;
        }

        #endregion

        #region Authentication

        public static WebApplicationBuilder AddIntuitiveAuthentication(this WebApplicationBuilder builder)
        {

            builder.Services.AddAuthentication("TenantAuthentication")
                .AddScheme<AuthenticationSchemeOptions, TenantAuthenticationHandler>("TenantAuthentication", null);

            builder.Services.AddAuthorization();

            return builder;
        }

        #endregion
    }
}
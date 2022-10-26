﻿namespace iVectorOne_Admin_Api.Security
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Options;
    using System.Security.Claims;
    using System.Text.Encodings.Web;

    public class TenantAuthorisationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ILogger<TenantAuthorisationHandler> _logger;
        private readonly ITenantService _tenantService;

        public TenantAuthorisationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ITenantService tenantService) : base(options, logger, encoder, clock)
        {
            _logger = logger.CreateLogger<TenantAuthorisationHandler>();
            _tenantService = tenantService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            AuthenticationTicket? ticket = null;
            string key = Request.Headers["TenantKey"];
            string errorMessage = "Invalid TenantKey";

            //Don't authenticate anonynous endpoints
           if (Context.GetEndpoint()?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(), Scheme.Name));
            }

            try
            {
                if (key != null)
                {
                    Guid tenantKey = new(key);
                    var tenant = await _tenantService.GetTenant(tenantKey);
                    if (tenant != null)
                    {
                        var authenticationIdentity = new TenantIdentity(tenant);
                        var claimsPrincipal = new ClaimsPrincipal(authenticationIdentity);
                        ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);
                    }
                }
                else
                {
                    errorMessage = "No TenantKey provided";
                }
            }
            catch (Exception ex)
            {
                ticket = null;
                errorMessage = ex.Message;
                _logger.LogError(ex, "Authentication Error");
            }

            if (ticket is not null)
            {
                return AuthenticateResult.Success(ticket);
            }
            else
            {
                Response.StatusCode = 401;
                Response.Headers.Add("Exception", errorMessage);
                return AuthenticateResult.Fail("Invalid TenantKey Header");
            }
        }
    }
}
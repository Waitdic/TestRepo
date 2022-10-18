using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace iVectorOne_Admin_Api.Security
{
    public class TenantAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ILogger<TenantAuthenticationHandler> _logger;
        private readonly ITenantService _tenantService;

        public TenantAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ITenantService tenantService) : base(options, logger, encoder, clock)
        {
            _logger = logger.CreateLogger<TenantAuthenticationHandler>();
            _tenantService = tenantService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            AuthenticationTicket? ticket = null;
            string key = Request.Headers["TenantKey"];

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
            }
            catch (Exception ex)
            {
                ticket = null;
                _logger.LogError(ex, "Authentication error");
            }

            if (ticket is not null)
            {
                return AuthenticateResult.Success(ticket);
            }
            else
            {
                Response.StatusCode = 401;
                return AuthenticateResult.Fail("Invalid or missing TenantKey header");
            }
        }
    }
}
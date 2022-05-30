namespace IVectorOne.Web.Infrastructure.Security
{
    using System;
    using System.Security.Claims;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Threading.Tasks;
    using IVectorOne.Web.Adaptors.Authentication;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IAuthenticationProvider _loginService;
        private readonly ILogger<BasicAuthenticationHandler> _logger;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock,
            IAuthenticationProvider loginService
            ) : base(options, loggerFactory, encoder, clock)
        {
            _loginService = loginService;
            _logger = loggerFactory.CreateLogger<BasicAuthenticationHandler>();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            AuthenticationTicket? ticket = null;
            string errorMessage = "Login details incorrect";

            try
            {
                string? authHeader = Request.Headers["Authorization"].ToString();

                if (authHeader != null && authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
                {
                    string? token = authHeader.Substring("Basic ".Length).Trim();

                    string? credentialstring = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                    string[]? credentials = credentialstring.Split(':');

                    var user = await _loginService.Authenticate(credentials[0], credentials[1]);

                    if (user != null)
                    {
                        var authenticationIdentity = new AuthenticationIdentity(user);
                        var claimsPrincipal = new ClaimsPrincipal(authenticationIdentity);
                        ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);
                    }
                }
            }
            catch (Exception e)
            {
                ticket = null;
                errorMessage = e.Message;
                _logger.LogError(e, "Authentication error");
            }

            if (ticket is not null)
            {
                return AuthenticateResult.Success(ticket);
            }
            else
            {
                Response.StatusCode = 401;
                Response.Headers.Add("Exception", errorMessage);
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 401;
            Response.ContentType = "application/json";

            var feature = Response.HttpContext.Features.Get<IHttpResponseBodyFeature>();
            if (feature is not null)
            {
                byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(new
                {
                    Message = "Failed to login: invalid credential"
                });

                await feature.Stream.WriteAsync(bytes);
            }
        }
    }
}
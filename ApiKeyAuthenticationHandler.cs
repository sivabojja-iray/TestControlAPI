using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace TestControlAPI
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string ApiKeyHeaderName = "X-Api-Key";
        private readonly string _apiKey;

        [Obsolete]
        public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _apiKey = "Your_Secret_Api_Key"; // This should be stored securely
        }
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing API Key"));
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if (_apiKey != providedApiKey)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
            }

            var claims = new[] { new Claim(ClaimTypes.Name, "APIKeyUser") };
            var identity = new ClaimsIdentity(claims, nameof(ApiKeyAuthenticationHandler));
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}

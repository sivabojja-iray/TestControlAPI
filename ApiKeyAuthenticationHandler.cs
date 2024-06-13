using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace TestControlAPI
{
    // Define a public class ApiKeyAuthenticationHandler that extends AuthenticationHandler<AuthenticationSchemeOptions>
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        // Define a constant string for the API Key header name
        public const string ApiKeyHeaderName = "X-Api-Key";

        // Declare a private readonly string to hold the API key
        private readonly string _apiKey;

        // Define a constructor with the Obsolete attribute, indicating it's an outdated method
        [Obsolete]
        public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)// Call the base constructor
        {
            // Assign a hardcoded API key to the private field (Note: should be stored securely)
            _apiKey = "Your_Secret_Api_Key"; // This should be stored securely
        }

        // Override the HandleAuthenticateAsync method to handle authentication logic
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Check if the request headers contain the API Key header
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
            {
                // If the header is missing, return a failed authentication result
                return Task.FromResult(AuthenticateResult.Fail("Missing API Key"));
            }

            // Get the first value of the API Key header
            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            // Compare the provided API key with the stored API key
            if (_apiKey != providedApiKey)
            {
                // If the keys don't match, return a failed authentication result
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
            }

            // Create claims for the authenticated user
            var claims = new[] { new Claim(ClaimTypes.Name, "APIKeyUser") };

            // Create a claims identity using the claims and the handler's name
            var identity = new ClaimsIdentity(claims, nameof(ApiKeyAuthenticationHandler));

            // Create an authentication ticket with the claims principal and scheme name
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);

            // Return a successful authentication result with the ticket
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
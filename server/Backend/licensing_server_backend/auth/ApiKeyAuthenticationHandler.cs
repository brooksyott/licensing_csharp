using Licensing.Common;
using Licensing.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Licensing.auth
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string ApiKeyHeaderName = "X-API-KEY";
        private readonly LicensingContext _dbContext;

        private static ConcurrentDictionary<string, string> _authKeyDict = new ConcurrentDictionary<string, string>();

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            LicensingContext context,
            UrlEncoder encoder) : base(options, logger, encoder)
        {
            _dbContext = context;
        }

        /// <summary>
        /// Fetches an internal auth key by key.
        /// </summary>
        /// <param name="key">Auth key.</param>
        /// <returns>Service result containing the internal auth key.</returns>
        public async Task<string> GetRoleAsync(string? key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    return "None";
                }

                if (_authKeyDict.TryGetValue(key, out var cachedRole))
                {
                    return cachedRole;
                }

                var authKey = await _dbContext.InternalAuthKeys.Where(x => x.Key == key).AsNoTracking().SingleOrDefaultAsync();
                if ((authKey == null) || (String.IsNullOrWhiteSpace(authKey.Role)))
                {
                    // Do not cache invalid keys. Caching them could lead to a DoS attack by filling up the cache and causing memory to overflow
                    return "None";
                }

                _authKeyDict[key] = authKey.Role;
                return authKey.Role;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error getting authKey by key: {key}");
                return "None";
            }
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                return AuthenticateResult.Fail("API Key is missing.");
            }

            var configuration = Context.RequestServices.GetRequiredService<IConfiguration>();

            if (string.IsNullOrEmpty(extractedApiKey))
            {
                return AuthenticateResult.Fail("API Key is missing.");
            }

            var role = await GetRoleAsync(extractedApiKey);
            if (role == "None")
            {
                return AuthenticateResult.Fail("Invalid API Key.");
            }

            // Create claims including the role
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "ApiKeyUser"),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}

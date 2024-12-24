using Licensing.Keys;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static Licensing.License.TokenService;
using Licensing.Common;

namespace Licensing.License
{

    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly IKeyService _keyService;

        public TokenService(ILogger<TokenService> logger, IKeyService keyService)
        {
            _logger = logger;
            _keyService = keyService;
        }

        public async Task<string> GenerateTokenAsync(string keyId, string email)
        {
            var key = await _keyService.DownloadPrivateKeyAsync(keyId);
            if (key == null || key.Data == null || key.Status != Common.ResultStatusCode.Success)
            {
                _logger.LogInformation("Failed to download private key for {keyId}", keyId);
                return null;
            }

            var rsa = RsaKeyLoader.LoadRsaPrivateKey(Encoding.UTF8.GetString(key.Data));

            var signingCredentials = new SigningCredentials(
                key: new RsaSecurityKey(rsa) { KeyId = keyId },
                algorithm: SecurityAlgorithms.RsaSha256
            );


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, "client-123")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = signingCredentials,

                // Here is where we add our custom "kid" header
                AdditionalHeaderClaims = new Dictionary<string, object>
                {
                    { "kid", keyId }
                }
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var newToken = tokenHandler.CreateToken(tokenDescriptor);
            var newJwt = tokenHandler.WriteToken(newToken);
            Console.WriteLine("Sencond Token: " + newJwt);

            await ValidateJwt(newJwt);

            return newJwt;
        }

        public async Task<string> ValidateJwt(string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.ReadJwtToken(jwtToken);
            object? kidValue;

            // 2) Retrieve the header, e.g., "kid"
            if (jwtSecurityToken.Header.TryGetValue("kid", out kidValue))
            {
                Console.WriteLine($"Header 'kid': {kidValue}");
            }

            if (kidValue == null)
            {
                return "No 'kid' found in JWT header.";
            }

            string? kid = kidValue.ToString();
            if (String.IsNullOrEmpty(kid))
            {
                return "No 'kid' found in JWT header.";
            }


            var getKeyResult = await _keyService.DownloadPublicKeyAsync(kid);
            if (getKeyResult == null || getKeyResult.Data == null || getKeyResult.Status != Common.ResultStatusCode.Success)
            {
                _logger.LogInformation("Failed to download private key for {keyId}", kid);
                return "Failed to download private key for " + kid;
            }

            var rsa = RsaKeyLoader.LoadRsaPublicKey(Encoding.UTF8.GetString(getKeyResult.Data));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = new RsaSecurityKey(rsa)
            };

            // Will throw an exception if invalid
            try
            {
                tokenHandler.ValidateToken(jwtToken, validationParameters, out var validatedToken);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "Ok";
        }
    }
}

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
using static Licensing.License.LicenseService;
using Licensing.Common;
using Newtonsoft.Json;
using Licensing.Data;
using Microsoft.EntityFrameworkCore;

namespace Licensing.License
{

    public class LicenseService : BaseService<LicenseService>, ILicenseService
    {
        private readonly IKeyService _keyService;

        public LicenseService(ILogger<LicenseService> logger, IKeyService keyService, LicensingContext context) : base(logger, context)
        {
            _keyService = keyService;
        }


        public async Task<ServiceResult<LicenseEntity>> DeleteLicenseAsync(string licenseId)
        {

            try
            {
                var toDeleteLicense = await _dbContext.Licenses.Where(x => x.Id == licenseId).AsNoTracking().SingleOrDefaultAsync();
                if (toDeleteLicense == null)
                {
                    return new ServiceResult<LicenseEntity>() { Status = ResultStatusCode.NotFound };
                }
                var returnedDeletedLicense = _dbContext.Licenses.Remove(toDeleteLicense);

                await _dbContext.SaveChangesAsync();
                return new ServiceResult<LicenseEntity>() { Status = ResultStatusCode.Success, Data = returnedDeletedLicense.Entity };
            }
            catch (Exception ex)
            {
                return ReturnException<LicenseEntity>(ex);
            }
        }

        public async Task<ServiceResult<PaginatedResults>> GetByCustomerIdAsync(string customerId, BasicQueryFilter filter)
        {
            try
            {
                //var licenses = await _dbContext.Licenses.Where(x => x.CustomerId == customerId).OrderBy(x => x.CreatedAt).Skip(filter.Offset).Take(filter.Limit).AsNoTracking().ToListAsync();
                List<LicenseDetailsEntity> licenses = await _dbContext.Licenses
                                    .Where(x => x.CustomerId == customerId)
                                    .GroupJoin(
                                        _dbContext.Customers,
                                        license => license.CustomerId, // Key from Licenses
                                        customer => customer.Id,       // Key from Customers
                                        (license, customers) => new { License = license, Customers = customers })
                                    .SelectMany(
                                        lc => lc.Customers.DefaultIfEmpty(), // Perform LEFT JOIN
                                        (lc, customer) => new LicenseDetailsEntity()
                                        {
                                            Id = lc.License.Id,
                                            CreatedAt = lc.License.CreatedAt,
                                            UpdatedAt = lc.License.UpdatedAt,
                                            Label = lc.License.Label,
                                            IssuedBy = lc.License.IssuedBy,
                                            License = lc.License.License,
                                            Description = lc.License.Description,
                                            Customer = new LicenseCustomerEntity()
                                            {
                                                Id = lc.License.CustomerId,
                                                Name = customer != null ? customer.Name : null // Handle nulls for unmatched customers
                                            }
                                        })
                                    .OrderBy(x => x.CreatedAt).Skip(filter.Offset).Take(filter.Limit).AsNoTracking().ToListAsync();


                // var licenses = await _dbContext.Licenses.OrderBy(x => x.CreatedAt).Skip(filter.Offset).Take(filter.Limit).AsNoTracking().ToListAsync();
                if (licenses == null)
                {
                    return new ServiceResult<PaginatedResults>()
                    {
                        Status = ResultStatusCode.Success,
                        Data = new PaginatedResults() { Limit = filter.Limit, Offset = filter.Offset, Results = new object[] { } }
                    };
                }

                foreach (var license in licenses)
                {
                    if (license.License == null)
                    {
                        continue;
                    }
                    license.Features = license.GetFeatures(license.License);
                }

                return new ServiceResult<PaginatedResults>()
                {
                    Status = ResultStatusCode.Success,
                    Data = new PaginatedResults() { Limit = filter.Limit, Offset = filter.Offset, Count = licenses.Count, Results = licenses }
                };
            }
            catch (Exception ex)
            {
                return ReturnException<PaginatedResults>(ex, "Error getting keys");
            }
        }

        public async Task<ServiceResult<LicenseDetailsEntity>> GetByIdAsync(string licenseId)
        {
            try
            {
                //var license = await _dbContext.Licenses.Where(x => x.Id == licenseId).AsNoTracking().SingleOrDefaultAsync();
                var license = await _dbContext.Licenses
                    .Where(x => x.Id == licenseId)
                    .GroupJoin(
                        _dbContext.Customers,
                        license => license.CustomerId, // Key from Licenses
                        customer => customer.Id,       // Key from Customers
                        (license, customers) => new { License = license, Customers = customers })
                    .SelectMany(
                        lc => lc.Customers.DefaultIfEmpty(), // Perform LEFT JOIN
                        (lc, customer) => new LicenseDetailsEntity()
                        {
                            Id = lc.License.Id,
                            CreatedAt = lc.License.CreatedAt,
                            UpdatedAt = lc.License.UpdatedAt,
                            Label = lc.License.Label,
                            IssuedBy = lc.License.IssuedBy,
                            License = lc.License.License,
                            Description = lc.License.Description,
                            Customer = new LicenseCustomerEntity()
                            {
                                Id = lc.License.CustomerId,
                                Name = customer != null ? customer.Name : null // Handle nulls for unmatched customers
                            }
                        })
                    .AsNoTracking().SingleOrDefaultAsync();
                if (license == null)
                {
                    return new ServiceResult<LicenseDetailsEntity>() { Status = ResultStatusCode.NotFound };
                }

                if (license.License != null)
                {
                    license.Features = license.GetFeatures(license.License);
                }

                return new ServiceResult<LicenseDetailsEntity>() { Status = ResultStatusCode.Success, Data = license };
            }
            catch (Exception ex)
            {
                return ReturnException<LicenseDetailsEntity>(ex, $"Error getting certificate information by Id");
            }
        }

        public async Task<ServiceResult<PaginatedResults>> GetLicensesAsync(BasicQueryFilter filter)
        {
            try
            {
                List<LicenseDetailsEntity> licenses = await _dbContext.Licenses
                                    .GroupJoin(
                                        _dbContext.Customers,
                                        license => license.CustomerId, // Key from Licenses
                                        customer => customer.Id,       // Key from Customers
                                        (license, customers) => new { License = license, Customers = customers })
                                    .SelectMany(
                                        lc => lc.Customers.DefaultIfEmpty(), // Perform LEFT JOIN
                                        (lc, customer) => new LicenseDetailsEntity()
                                        {
                                            Id = lc.License.Id,
                                            CreatedAt = lc.License.CreatedAt,
                                            UpdatedAt = lc.License.UpdatedAt,
                                            Label = lc.License.Label,
                                            IssuedBy = lc.License.IssuedBy,
                                            License = lc.License.License,
                                            Description = lc.License.Description,
                                            Customer = new LicenseCustomerEntity()
                                            {
                                                Id = lc.License.CustomerId,
                                                Name = customer != null ? customer.Name : null // Handle nulls for unmatched customers
                                            }
                                        })
                                    .OrderBy(x => x.CreatedAt).Skip(filter.Offset).Take(filter.Limit).AsNoTracking().ToListAsync();


                // var licenses = await _dbContext.Licenses.OrderBy(x => x.CreatedAt).Skip(filter.Offset).Take(filter.Limit).AsNoTracking().ToListAsync();
                if (licenses == null)
                {
                    return new ServiceResult<PaginatedResults>()
                    {
                        Status = ResultStatusCode.Success,
                        Data = new PaginatedResults() { Limit = filter.Limit, Offset = filter.Offset, Results = new object[] { } }
                    };
                }

                foreach (var license in licenses)
                {
                    if (license.License == null)
                    {
                        continue;
                    }
                    license.Features = license.GetFeatures(license.License);
                }

                return new ServiceResult<PaginatedResults>()
                {
                    Status = ResultStatusCode.Success,
                    Data = new PaginatedResults() { Limit = filter.Limit, Offset = filter.Offset, Count = licenses.Count, Results = licenses }
                };
            }
            catch (Exception ex)
            {
                return ReturnException<PaginatedResults>(ex, $"Error getting keys");
            }
        }

        public async Task<ServiceResult<LicenseEntity>> GenerateLicenseAsync(GenerateLicenseRequestBody licenseRequest)
        {
            if ((licenseRequest == null) || (licenseRequest.IsValid() == false))
            {
                return new ServiceResult<LicenseEntity>
                {
                    Status = ResultStatusCode.BadRequest,
                    ErrorMessage = new ErrorMessageStruct("Invalid request body")
                };
            }

            if (licenseRequest.Features == null || licenseRequest.Features.Count == 0)
            {
                return new ServiceResult<LicenseEntity>
                {
                    Status = ResultStatusCode.BadRequest,
                    ErrorMessage = new ErrorMessageStruct("Features are required")
                };
            }

            // Build the list of SKUs
            licenseRequest.Features = licenseRequest.Features.GroupBy(x => x.Sku).Select(y => y.First()).ToList();
            var skuList = licenseRequest.Features.Select(f => f.Sku).Distinct().ToList();

            var skus = await _dbContext.Skus
               .Where(s => skuList.Contains(s.SkuCode))
               .AsNoTracking().ToListAsync();

            // Found less SKUs than requested
            if (skus == null || skus.Count < skuList.Count)
            {
                return new ServiceResult<LicenseEntity>
                {
                    Status = ResultStatusCode.BadRequest,
                    ErrorMessage = new ErrorMessageStruct("Invalid features")
                };
            }

            var featuresJson = JsonConvert.SerializeObject(licenseRequest.Features);

            var key = await _keyService.DownloadPrivateKeyAsync(licenseRequest.KeyId);
            if (key == null || key.Data == null || key.Status != Common.ResultStatusCode.Success)
            {
                _logger.LogInformation("Failed to download private key for {keyId}", licenseRequest.KeyId);
                return new ServiceResult<LicenseEntity>
                {
                    Status = ResultStatusCode.BadRequest,
                    ErrorMessage = new ErrorMessageStruct($"Failed to download private key for {licenseRequest.KeyId}")
                };
            }

            var rsa = PemUtils.LoadRsaPrivateKey(Encoding.UTF8.GetString(key.Data));

            var signingCredentials = new SigningCredentials(
                key: new RsaSecurityKey(rsa) { KeyId = licenseRequest.KeyId },
                algorithm: SecurityAlgorithms.RsaSha256
            );

            if (String.IsNullOrWhiteSpace(licenseRequest?.CustomerId) || String.IsNullOrWhiteSpace(licenseRequest?.IssuedBy))
            {
                return new ServiceResult<LicenseEntity>
                {
                    Status = ResultStatusCode.BadRequest,
                    ErrorMessage = new ErrorMessageStruct("CustomerId is required")
                };
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Aud, licenseRequest.CustomerId),
                    new Claim(JwtRegisteredClaimNames.Sub, "JSI License"),
                    new Claim(JwtRegisteredClaimNames.Iss, licenseRequest.IssuedBy),
                    new Claim("features", featuresJson),
                }),

                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = signingCredentials
            };



            var tokenHandler = new JwtSecurityTokenHandler();
            var newToken = tokenHandler.CreateToken(tokenDescriptor);
            var newJwt = tokenHandler.WriteToken(newToken);
            _logger.LogInformation($"Token created for {licenseRequest.CustomerId} using key {licenseRequest.KeyId} by {licenseRequest.IssuedBy}");

            (bool rc, string message) = await ValidateJwt(newJwt);
            if (rc == false)
            {
                return new ServiceResult<LicenseEntity>
                {
                    Status = ResultStatusCode.InternalServerError,
                    ErrorMessage = new ErrorMessageStruct(message)
                };
            }

            try
            {

                var addEntity = _dbContext.Licenses.Add(new LicenseEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Label = licenseRequest.Label,
                    IssuedBy = licenseRequest.IssuedBy,
                    License = newJwt,
                    Description = licenseRequest.Description,
                    CustomerId = licenseRequest.CustomerId,
                });

                await _dbContext.SaveChangesAsync();

                return new ServiceResult<LicenseEntity>
                {
                    Status = ResultStatusCode.Success,
                    Data = addEntity.Entity
                };
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException == null)
                {
                    return ReturnException<LicenseEntity>(dbEx, "Error adding license to database");
                }

                // Could check in more detail which constraint was violated
                if (dbEx.InnerException.Message.Contains("foreign"))
                {
                    return new ServiceResult<LicenseEntity>
                    {
                        Status = ResultStatusCode.BadRequest,
                        ErrorMessage = new ErrorMessageStruct("Foreign key exception, please check customer exists and/or other constraints")
                    };
                }
                return ReturnException<LicenseEntity>(dbEx, "Error adding license to database");
            }
            catch (Exception ex)
            {
                return ReturnException<LicenseEntity>(ex, "Error adding license to database");
            }

        }

        public async Task<(bool, string)> ValidateJwt(string jwtToken)
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
                string errorMessage = "No 'kid' found in JWT header.";
                _logger.LogInformation(errorMessage);
                return (false, errorMessage);
            }

            string? kid = kidValue.ToString();
            if (String.IsNullOrEmpty(kid))
            {
                string errorMessage = "No 'kid' found in JWT header.";
                _logger.LogInformation(errorMessage);
                return (false, errorMessage);
            }


            var getKeyResult = await _keyService.DownloadPublicKeyAsync(kid);
            if (getKeyResult == null || getKeyResult.Data == null || getKeyResult.Status != Common.ResultStatusCode.Success)
            {
                string errorMessage = $"Failed to download public key for {kid}";
                _logger.LogInformation(errorMessage);
                return (false, errorMessage);
            }

            var rsa = PemUtils.LoadRsaPublicKey(Encoding.UTF8.GetString(getKeyResult.Data));

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
                var featuresClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "features")?.Value;
                if (featuresClaim == null)
                {
                    return (false, "No 'features' claim found in JWT.");
                }
                var features = JsonConvert.DeserializeObject<List<Feature>>(featuresClaim);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }

            return (true, "Ok");
        }

        public static string DecodeJwtToString(string jwt)
        {
            // 1) Split the token by '.'
            //    A JWT should have 3 parts: header, payload, signature
            var parts = jwt.Split('.');
            if (parts.Length < 2)
            {
                throw new ArgumentException("Invalid JWT format. Expected at least 2 parts (header, payload).");
            }

            // 2) Decode the header (parts[0]) and payload (parts[1]) from Base64URL
            string headerJson = Base64UrlDecodeToString(parts[0]);
            string payloadJson = Base64UrlDecodeToString(parts[1]);

            // 3) Create a human-readable output
            //    You can also return a custom object or two separate strings, if desired
            return
                headerJson +
                payloadJson;
        }

        /// <summary>
        /// Decodes a Base64Url-encoded string into a UTF8 string.
        /// Base64Url replaces '+' with '-', '/' with '_', and omits padding.
        /// </summary>
        private static string Base64UrlDecodeToString(string base64Url)
        {
            // 1) Convert from base64url to normal base64
            int mod4 = base64Url.Length % 4;
            if (mod4 == 2)
            {
                base64Url += "==";
            }
            else if (mod4 == 3)
            {
                base64Url += "=";
            }

            // 2) Convert Base64Url to standard Base64
            base64Url = base64Url.Replace('-', '+').Replace('_', '/');

            // 3) Decode to bytes
            byte[] data = Convert.FromBase64String(base64Url);

            // 4) Convert to UTF-8 text
            return System.Text.Encoding.UTF8.GetString(data);
        }


    }
}

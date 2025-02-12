using Licensing.Common;
using Licensing.Customers;
using Licensing.Data;
using Licensing.Keys;
using Licensing.Skus;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Licensing.License
{
    /// <summary>
    /// Service for managing licenses.
    /// </summary>
    public class LicenseService : BaseService<LicenseService>, ILicenseService
    {
        private readonly IKeyService _keyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="keyService">The key service instance.</param>
        /// <param name="context">The database context.</param>
        public LicenseService(ILogger<LicenseService> logger, IKeyService keyService, LicensingContext context) : base(logger, context)
        {
            _keyService = keyService;
        }

        /// <summary>
        /// Generates a new license.
        /// </summary>
        /// <param name="licenseRequest">The license generation request body.</param>
        /// <returns>Service result containing the generated license entity.</returns>
        public async Task<ServiceResult<LicenseEntity>> GenerateLicenseAsync(GenerateLicenseRequestBody licenseRequest)
        {
            if (licenseRequest == null || String.IsNullOrEmpty(licenseRequest.IssuedBy) || String.IsNullOrEmpty(licenseRequest.CustomerId) || String.IsNullOrEmpty(licenseRequest.KeyId))
            {
                return new ServiceResult<LicenseEntity>
                {
                    Status = ResultStatusCode.BadRequest,
                    ErrorMessage = new ErrorInformation("Invalid request body")
                };
            }

            if (licenseRequest.Features == null || licenseRequest.Features.Count == 0)
            {
                return new ServiceResult<LicenseEntity>
                {
                    Status = ResultStatusCode.BadRequest,
                    ErrorMessage = new ErrorInformation("Features are required")
                };
            }

            // Validate the skus being requested
            (var skuRequest, var skus) = await ValidateSkuList(licenseRequest);
            if (skuRequest.Status != ResultStatusCode.Success)
            {
                return skuRequest;
            }

            var jwtId = Guid.NewGuid().ToString();

            // Create the JWT
            (var jwtRequest, var newJwt) = await BuildJWT(jwtId, licenseRequest);
            if (jwtRequest.Status != ResultStatusCode.Success)
            {
                return jwtRequest;
            }

            try
            {
                var addEntity = _dbContext.Licenses.Add(new LicenseEntity
                {
                    Id = jwtId,
                    Label = licenseRequest.Label,
                    IssuedBy = licenseRequest.IssuedBy,
                    License = newJwt,
                    KeyId = licenseRequest.KeyId,
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
            catch (Exception ex)
            {
                return ReturnException<LicenseEntity>(ex, "Error adding license to database");
            }
        }

        public async Task<ServiceResult<LicenseEntity>> UpdateLicenseAsync(string licenseId, UpdateLicenseRequestBody licenseRequest)
        {
            try
            {
                // Fetch the existing customer from the database by ID
                var updatedLicense = await _dbContext.Licenses.Where(x => x.Id == licenseId).AsNoTracking().SingleOrDefaultAsync();
                if (updatedLicense == null)
                {
                    // Return a not found result if the customer does not exist
                    return new ServiceResult<LicenseEntity>() { Status = ResultStatusCode.NotFound };
                }

                // Update the customer data
                updatedLicense.Label = licenseRequest.Label;
                updatedLicense.Description = licenseRequest.Description;
                _dbContext.Licenses.Update(updatedLicense);
                var dbResponse = await _dbContext.SaveChangesAsync();
                // Return the updated customer data
                return new ServiceResult<LicenseEntity>() { Status = ResultStatusCode.Success, Data = updatedLicense };
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error result
                return ReturnException<LicenseEntity>(ex, $"Error updating customer");
            }
        }

        /// <summary>
        /// Deletes a license by ID.
        /// </summary>
        /// <param name="licenseId">The license ID.</param>
        /// <returns>Service result containing the deleted license entity.</returns>
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

        /// <summary>
        /// Fetches licenses by customer ID with pagination.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="filter">Pagination filter.</param>
        /// <returns>Service result containing paginated list of licenses.</returns>
        public async Task<ServiceResult<PaginatedResults>> GetByCustomerIdAsync(string customerId, BasicQueryFilter filter)
        {
            try
            {
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
                                            KeyId = lc.License.KeyId,
                                            Customer = new LicenseCustomerEntity()
                                            {
                                                Id = lc.License.CustomerId,
                                                Name = customer != null ? customer.Name : null // Handle nulls for unmatched customers
                                            }
                                        })
                                    .OrderBy(x => x.CreatedAt).Skip(filter.Offset).Take(filter.Limit).AsNoTracking().ToListAsync();

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

        /// <summary>
        /// Fetches a license by ID.
        /// </summary>
        /// <param name="licenseId">The license ID.</param>
        /// <returns>Service result containing the license details entity.</returns>
        public async Task<ServiceResult<LicenseDetailsEntity>> GetByIdAsync(string licenseId)
        {
            try
            {
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
                            KeyId = lc.License.KeyId,
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

        /// <summary>
        /// Fetches a paginated list of licenses.
        /// </summary>
        /// <param name="filter">Pagination filter.</param>
        /// <returns>Service result containing paginated list of licenses.</returns>
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
                                            KeyId = lc.License.KeyId,
                                            Customer = new LicenseCustomerEntity()
                                            {
                                                Id = lc.License.CustomerId,
                                                Name = customer != null ? customer.Name : null // Handle nulls for unmatched customers
                                            }
                                        })
                                    .OrderBy(x => x.CreatedAt).Skip(filter.Offset).Take(filter.Limit).AsNoTracking().ToListAsync();

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

        /// <summary>
        /// Builds the list of SKUs from the license request.
        /// </summary>
        /// <param name="licenseRequest"></param>
        /// <returns></returns>
        private async Task<(ServiceResult<LicenseEntity>, List<SkuEntity>?)> ValidateSkuList(GenerateLicenseRequestBody licenseRequest)
        {
            if (licenseRequest.Features == null || licenseRequest.Features.Count == 0)
            {
                return (new ServiceResult<LicenseEntity>
                {
                    Status = ResultStatusCode.BadRequest,
                    ErrorMessage = new ErrorInformation("Features are required")
                }, null);
            }

            // Build the list of SKUs
            licenseRequest.Features = licenseRequest.Features.GroupBy(x => x.Sku).Select(y => y.First()).ToList();
            var skuList = licenseRequest.Features.Select(f => f.Sku).Distinct().ToList();

            var skus = await _dbContext.Skus
               .Where(s => skuList.Contains(s.Id))
               .AsNoTracking().ToListAsync();

            // Found less SKUs than requested
            if (skus == null || skus.Count < skuList.Count)
            {
                return (new ServiceResult<LicenseEntity>
                {
                    Status = ResultStatusCode.BadRequest,
                    ErrorMessage = new ErrorInformation("Invalid features")
                }, null);
            }

            return (new ServiceResult<LicenseEntity>
            {
                Status = ResultStatusCode.Success
            }, skus);
        }

        /// <summary>
        /// Builds a JWT token for the license request.
        /// </summary>
        /// <param name="licenseRequest"></param>
        /// <returns></returns>
        private async Task<(ServiceResult<LicenseEntity>, string)> BuildJWT(string id, GenerateLicenseRequestBody licenseRequest)
        {
            var key = await _keyService.DownloadPrivateKeyAsync(licenseRequest.KeyId);
            if (key == null || key.Data == null || key.Status != Common.ResultStatusCode.Success)
            {
                _logger.LogInformation("Failed to download private key for {keyId}", licenseRequest.KeyId);
                return (new ServiceResult<LicenseEntity>
                {
                    Status = ResultStatusCode.BadRequest,
                    ErrorMessage = new ErrorInformation($"Failed to download private key for {licenseRequest.KeyId}")
                }, String.Empty);
            }
            var rsa = PemUtils.LoadRsaPrivateKey(Encoding.UTF8.GetString(key.Data));

            var signingCredentials = new SigningCredentials(
                key: new RsaSecurityKey(rsa) { KeyId = licenseRequest.KeyId },
                algorithm: SecurityAlgorithms.RsaSha256
            );

            if (string.IsNullOrWhiteSpace(licenseRequest?.CustomerId) || string.IsNullOrWhiteSpace(licenseRequest?.IssuedBy))
            {
                return (new ServiceResult<LicenseEntity>
                {
                    Status = ResultStatusCode.BadRequest,
                    ErrorMessage = new ErrorInformation("CustomerId is required")
                }, String.Empty);
            }

            var featuresJson = JsonConvert.SerializeObject(licenseRequest.Features);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Aud, licenseRequest.CustomerId),
                    new Claim(JwtRegisteredClaimNames.Sub, "JSI License"),
                    new Claim(JwtRegisteredClaimNames.Iss, licenseRequest.IssuedBy),
                    new Claim(JwtRegisteredClaimNames.Jti, id),
                    new Claim("features", featuresJson),
                }),

                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var newToken = tokenHandler.CreateToken(tokenDescriptor);
            var newJwt = tokenHandler.WriteToken(newToken);


            (bool rc, string message) = await ValidateJwt(newJwt);
            if (rc == false)
            {
                return (new ServiceResult<LicenseEntity>
                {
                    Status = ResultStatusCode.InternalServerError,
                    ErrorMessage = new ErrorInformation(message)
                }, string.Empty);
            }

            return (new ServiceResult<LicenseEntity>
            {
                Status = ResultStatusCode.Success
            }, newJwt);
        }

        /// <summary>
        /// Validates a JWT token.
        /// </summary>
        /// <param name="jwtToken">The JWT token.</param>
        /// <returns>A tuple containing a boolean indicating success and a message.</returns>
        private async Task<(bool, string)> ValidateJwt(string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.ReadJwtToken(jwtToken);
            object? kidValue;

            // Retrieve the header, e.g., "kid"
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
            if (string.IsNullOrEmpty(kid))
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
    }
}


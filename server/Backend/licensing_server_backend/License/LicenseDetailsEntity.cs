using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using Licensing.Customers;

namespace Licensing.License
{
    /// <summary>
    /// Represents a license details entity.
    /// Additionally to what is stored in the database, it contains a list of features and a customer details.
    /// The license/token does have this information, but it's more user and UI friendly to have it here as well.
    /// </summary>
    public class LicenseDetailsEntity
    {

        public string? Id { get; set; }
        public DateTime CreatedAt { get;  set; }
        public DateTime UpdatedAt { get;  set; }
        public string? Label { get; set; }
        public string? IssuedBy { get; set; }
        public string? License { get; set; }
        public string? Description { get; set; }
        public LicenseCustomerEntity? Customer { get; set; }

        public LicenseDetailsEntity() { } 

        public LicenseDetailsEntity(LicenseEntity license)
        { 
            this.Id = license.Id;
            this.CreatedAt = license.CreatedAt;
            this.UpdatedAt = license.UpdatedAt;
            this.IssuedBy = license.IssuedBy;
            this.License = license.License;
            this.Description = license.Description;
            Customer = new LicenseCustomerEntity();
        }

        /// <summary>
        /// Get the features from the license token.
        /// </summary>
        /// <param name="license"></param>
        /// <returns></returns>
        public List<Feature>? GetFeatures(string license)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = tokenHandler.ReadJwtToken(this.License);
                string? featuresClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "features")?.Value;
                if (String.IsNullOrEmpty(featuresClaim))
                {
                    return null;
                }
                return JsonConvert.DeserializeObject<List<Feature>>(featuresClaim);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Feature>? Features { get; set; }    
    }
}

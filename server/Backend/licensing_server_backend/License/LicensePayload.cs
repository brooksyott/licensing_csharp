/// <summary>
/// This file contains the classes that represent the license information which is containted within the JWT token as a claim
/// </summary>
namespace Licensing.License
{
    public class RateLimit
    {
        public string? Name { get; set; }  // e.g. "API calls"
        public int Limit { get; set; }    // e.g. 1000 calls
        public int Period { get; set; }   // e.g. 3600 seconds (1 hour)
    }

    /// <summary>
    /// Represents a feature that is part of the license
    /// The Sku is a unique identifier for the feature
    /// Expiry is the unix timestamp when the feature expires
    /// </summary>
    public class Feature
    {
        public string? Sku { get; set; }
        public long Expiry { get; set; }
        public List<RateLimit>? RateLimits { get; set; }
    }

    /// <summary>
    /// A list of features that are part of the license. This is the root of the claim
    /// </summary>
    public class LicensePayload
    {
        public List<Feature>? Features { get; set; }
    }
}

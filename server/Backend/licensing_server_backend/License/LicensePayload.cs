namespace Licensing.License
{
    public class RateLimit
    {
        public string? Name { get; set; }  // e.g. "API calls"
        public int Limit { get; set; }    // e.g. 1000 calls
        public int Period { get; set; }   // e.g. 3600 seconds (1 hour)
    }

    public class Feature
    {
        public string? Sku { get; set; }
        public long Expiry { get; set; }
        public List<RateLimit>? RateLimits { get; set; }
    }

    public class LicensePayload
    {
        public List<Feature>? Features { get; set; }
    }
}

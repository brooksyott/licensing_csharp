namespace Licensing.License
{
    /// <summary>
    /// Request body for generating a license.
    /// </summary>
    public class GenerateLicenseRequestBody
    {
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(KeyId) && !string.IsNullOrWhiteSpace(IssuedBy) && !string.IsNullOrWhiteSpace(CustomerId) && !string.IsNullOrWhiteSpace(Label) && Features != null;
        }

        public string? KeyId { get; set; }
        public string? IssuedBy { get; set; }
        public string? CustomerId { get; set; }
        public string? Label { get; set; }
        public string? Description { get; set; }
        public List<Feature>? Features { get; set; }
    }
}

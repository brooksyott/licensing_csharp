namespace Licensing.Keys
{
    /// <summary>
    /// Request body for generating a new key
    /// </summary>
    public class KeyGenerationRequestBody
    {
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Label) && 
                   !string.IsNullOrWhiteSpace(CreatedBy) && 
                   !string.IsNullOrWhiteSpace(UpdatedBy);
        }

        public string? Label { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Description { get; set; }
    }
}

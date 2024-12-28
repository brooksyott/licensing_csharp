namespace Licensing.Keys
{
    // Request body for updating a key
    public class KeyUpdateRequestBody
    {
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Label) &&
                   !string.IsNullOrWhiteSpace(UpdatedBy);
        }

        public string? Label { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Description { get; set; }
    }
}

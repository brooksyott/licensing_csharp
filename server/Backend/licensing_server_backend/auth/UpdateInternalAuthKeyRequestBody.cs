namespace Licensing.auth
{
    /// <summary>
    /// Request body for updating an internal auth key.
    /// </summary>
    public class UpdateInternalAuthKeyRequestBody
    {
        public string? Role { get; set; }
        public string? Description { get; set; }
    }
}

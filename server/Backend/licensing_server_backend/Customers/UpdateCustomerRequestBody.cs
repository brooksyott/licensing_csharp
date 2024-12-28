namespace Licensing.Customers
{
    /// <summary>
    /// Request body for updating a customer.
    /// </summary>
    public class UpdateCustomerRequestBody
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public required bool IsDeleted { get; set; }
    }
}

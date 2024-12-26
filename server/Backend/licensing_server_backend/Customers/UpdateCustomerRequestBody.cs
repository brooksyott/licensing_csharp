namespace Licensing.Customers
{
    public class UpdateCustomerRequestBody
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public required bool IsDeleted { get; set; }
    }
}

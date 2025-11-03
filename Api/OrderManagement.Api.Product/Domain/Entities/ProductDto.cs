namespace OrderManagement.Api.Products.Domain.Entities
{
    public class ProductDto
    {
        public required string Name { get; set; }
        public required int CategoryId { get; set; }
    }
}

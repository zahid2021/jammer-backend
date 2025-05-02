namespace Jammer.ProductModule.DTOs
{
    public class ProductUpdateRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? ParentId { get; set; }
    }
}

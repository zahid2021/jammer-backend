namespace Jammer.ProductModule.Models
{
    public class ProductImage
    {
        public  int Id { get; set; }
        public required int ProductId { get; set; }
        public required string ImagePath { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}

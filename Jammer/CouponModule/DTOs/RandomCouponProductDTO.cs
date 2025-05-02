namespace Jammer.OrderModule.DTOs
{
    public class RandomCouponProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string ProductURL { get; set; }

        public string CouponId { get; set; }
        public decimal Discount { get; set; }
        public string DiscountType { get; set; }
        public DateTime ExpirationDate { get; set; }
        public decimal DiscountedPrice { get; set; }
        public List<string> ImagePaths { get; set; } = new List<string>(); 
    }
    public class ProductImageDTO
    {
        public int ProductId { get; set; }
        public string ImagePath { get; set; }
    }
    public class ProductListDTOs
    {
        public List<int> id { get; set; }
        public ProductListDTOs(List<int>? data)
        {
            if (data == null)
                id = [];
            else
                id = data;
        }
    }public class GetProductsByDiscountRange
    {
        public decimal minDiscount { get; set; }
        public decimal maxDiscount { get; set; }
        public GetProductsByDiscountRange(decimal minDiscount1, decimal maxDiscount1)
        {
            maxDiscount = maxDiscount1;
            minDiscount = minDiscount1;
        }
    }
}

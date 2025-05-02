namespace Jammer.OrderModule.DTOs
{
    public class CouponProductDTO
    {
        public int CouponId { get; set; }
        public int ProductId { get; set; }
        public decimal DiscountAmount { get; set; } 
        public decimal DiscountPercentage { get; set; } 
        public string DiscountType { get; set; }
    }
}

namespace Jammer.CouponModule.DTOs
{
    public class CouponResponseDTO
    {
        public decimal DiscountAmount { get; set; }
        public string DiscountType { get; set; }
        public decimal FinalPrice { get; set; }
    }
}

namespace Jammer.CouponModule.DTOs
{
    public class ApplyCouponRequestDTO

    {
        public string Id { get; set; }
        public int? ProductId { get; set; }
        public int? CategoryId { get; set; }
    }
}

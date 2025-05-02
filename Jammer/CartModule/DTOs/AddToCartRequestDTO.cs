namespace Jammer.CartModule.DTOs
{
    public class AddToCartRequestDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int CouponId { get; set; }
    }
    public class updateItem
    {
        public int CartId { get; set; }
        public int Quantity { get; set; }
    }
 public class UpdateCartRequestDTO
    {
        public List<updateItem> Items { get; set; } = new List<updateItem>();
    }
}

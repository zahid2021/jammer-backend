namespace Jammer.OrderModule.DTOs
{
    public class OrderResponse
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
        public List<OrderItemDTO>? Items { get; set; }
    }
}namespace Jammer.OrderModule.DTOs
{
    public class AllOrderResponse
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
        public string fullname { get; set; }
        public List<OrderItemDTO>? Items { get; set; }
    }
}

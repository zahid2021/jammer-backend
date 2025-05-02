namespace Jammer.OrderModule.DTOs
{
    public class DeliveryOrderDTO
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
        public string FullName { get; set; } 
    }

}

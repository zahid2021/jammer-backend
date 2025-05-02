namespace Jammer.OrderModule.DTOs
{
    public class OrderDTO
    {
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
    }

}

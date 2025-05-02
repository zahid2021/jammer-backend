using Jammer.OrderModule.DTOs;

namespace Jammer.OrderModule.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }
   
        public class ALLOrderDTO
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string City { get; set; }
            public string Street { get; set; }
            public string PostalCode { get; set; }
            public string Region { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal TotalAmount { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Status { get; set; }
            public List<OrderItemDTO> Items { get; set; } 
        }

}

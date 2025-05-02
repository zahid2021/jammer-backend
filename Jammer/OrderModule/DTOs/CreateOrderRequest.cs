using System.ComponentModel.DataAnnotations;

namespace Jammer.OrderModule.DTOs
{
    public class CreateOrderRequest
    {
        //[Required]
        //public int UserId { get; set; }
      
            public string City { get; set; }
            public string Street { get; set; }
            public string PostalCode { get; set; }
            public string Region { get; set; }
     

        [Required]
        [MinLength(1, ErrorMessage = "At least one order item is required.")]
        public List<OrderItem> Items { get; set; }
    }
    public class CreateOrder
    {
        
            public string Status { get; set; }
            public int OrderId { get; set; }
        public CreateOrder(string status, int orderId)
        {
            Status = status;
            OrderId = orderId;
        }
    }


}

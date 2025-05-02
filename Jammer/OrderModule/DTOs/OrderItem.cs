using System.ComponentModel.DataAnnotations;

namespace Jammer.OrderModule.DTOs
{
    public class OrderItem
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        public int? CouponId { get; set; }
    }
    
        public class OrderItemDTO
        {
            public int Id { get; set; }
            public int OrderId { get; set; }
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        public string ProductURL { get; set; }

        public string ProductName { get; set; }
            public string ProductImagePath { get; set; }
        }
    public class UserDTO
    {
        public int Id { get; set; }         
        public string fullname { get; set; }  
    }




}

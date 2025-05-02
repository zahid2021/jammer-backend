using Jammer.CartModule.DTOs;

namespace Jammer.CartModule.Models
{
    public class CartDTO
    {
        public int UserId { get; set; }
        public List<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();
    }
}

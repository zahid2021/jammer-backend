using Jammer.CartModule.DTOs;
using Jammer.ProductModule.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Jammer.CartModule.Repositories.InterFace
{
    public interface ICartRepositories

    {
        Task<bool> CreateCart(AddToCartRequestDTO request, string id);
        Task<List<CartItemDTO>> GetUserCart(string userId);
        Task<CartItemDTO> GetCartById(string userId, int cartId);
        Task<bool> DeleteCartItem(int cartId);
        Task<bool> DeleteAllUserCart(string userid);
        Task<bool> UpdateCartItems(UpdateCartRequestDTO model);
    }
}

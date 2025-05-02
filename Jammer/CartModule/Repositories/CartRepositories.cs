using Dapper;
using Jammer.CartModule.DTOs;
using Jammer.CartModule.Repositories.InterFace;
using Jammer.CouponModule.Models;
using Jammer.DBContext;
using Jammer.OrderModule.DTOs;
using System.Data;

namespace Jammer.CartModule.Repositories
{
    public class CartRepositories : ICartRepositories
    {
        readonly IDapperContext _context;

        public CartRepositories(IDapperContext context)
        {

            _context = context;
        }
        public async Task<bool> CreateCart(AddToCartRequestDTO request,string id)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = $@"
            INSERT INTO Cart (UserId, ProductId,CouponId ,Quantity) 
            VALUES ({id}, @ProductId,@CouponId, @Quantity);";


                int rowsAffected =await db.ExecuteAsync(query, request);


                if (rowsAffected == 0)
                return false;

            return true;

        }

        public async Task<List<CartItemDTO>> GetUserCart(string userId)
        {
            using IDbConnection db = _context.CreateConnection();

            string cartQuery = @"
    SELECT 
        c.Id, 
        c.ProductId, 
        p.Name AS ProductName, 
        p.ProductURL,
        p.Price, 
        c.CouponId, 
        c.Quantity, 
        c.CreatedAt
    FROM 
        Cart c
    LEFT JOIN 
        Product p ON c.ProductId = p.Id
    WHERE 
        c.UserId = @id;";

            var cartItems = (await db.QueryAsync<CartItemDTO>(cartQuery, new StringDTOs(userId))).ToList();

            if (!cartItems.Any())
                return cartItems;

            var productIds = cartItems.Select(ci => ci.ProductId).Distinct().ToList();

            string imageQuery = @"
    SELECT 
        pi.ProductId, 
        pi.ImagePath
    FROM 
        ProductImage pi
    WHERE 
        pi.ProductId IN @id;";

            var productImages = (await db.QueryAsync<ProductImageDTO>(imageQuery, new ProductListDTOs(productIds))).ToList();

            var productImagesLookup = productImages.ToLookup(pi => pi.ProductId);

            foreach (var cartItem in cartItems)
            {
                cartItem.ProductImages = productImagesLookup[cartItem.ProductId].Select(pi => pi.ImagePath).ToList();
            }

            return cartItems;
        }


        public async Task<CartItemDTO?> GetCartById(string userId, int cartId)
        {
            using IDbConnection db = _context.CreateConnection();

            string cartQuery = @"
    SELECT 
        c.Id, 
        c.ProductId, 
        p.Name AS ProductName, 
        p.Price, 
        p.ProductURL,
        c.CouponId,
        c.Quantity, 
        c.CreatedAt
    FROM 
        Cart c
    LEFT JOIN 
        Product p ON c.ProductId = p.Id
    WHERE 
        c.Id = @CartId ;"; 

            var cartItem = (await db.QueryAsync<CartItemDTO>(cartQuery, new GetCartByIdDTO(userId, cartId ))).FirstOrDefault();

            if (cartItem == null)
                return null;
            string imageQuery = @"
    SELECT 
        pi.ProductId, 
        pi.ImagePath
    FROM 
        ProductImage pi
    WHERE 
        pi.ProductId = @id;"; 
            var productImages = await db.QueryAsync<ProductImageDTO>(imageQuery, new { id = cartItem.ProductId });

            cartItem.ProductImages = productImages.Select(pi => pi.ImagePath).ToList();

            return cartItem;
        }


        public async Task<bool> DeleteCartItem(int cartId)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = "DELETE FROM Cart WHERE Id = @CartId;";

            
                int rowsAffected = await db.ExecuteAsync(query, new { CartId = cartId });


                if (rowsAffected == 0)
                return false;

            return true;


        } 
        public async Task<bool> DeleteAllUserCart(string userid)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = "DELETE FROM Cart WHERE UserId = @UserId;";

            
                int rowsAffected = await db.ExecuteAsync(query, new { UserId = userid });


                if (rowsAffected == 0)
                return false;

            return true;


        }
        public async Task<bool> UpdateCartItems(UpdateCartRequestDTO model)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = "UPDATE Cart SET Quantity = @Quantity WHERE Id = @CartId;";

            int totalRowsAffected = 0;

            foreach (var item in model.Items)
            {
                int rowsAffected = await db.ExecuteAsync(query, new { Quantity = item.Quantity, CartId = item.CartId });
                totalRowsAffected += rowsAffected;
            }

            return totalRowsAffected > 0;
        }
    }
}

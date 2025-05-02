using Jammer.CartModule.DTOs;
using Jammer.DBContext;
using System.Data;
using Dapper;
using Jammer.CartModule.Repositories.InterFace;
using Jammer.CouponModule.Models;
using Jammer.OrderModule.DTOs;

namespace Jammer.CartModule.Repositories
{
    public class WishListRepositories : IWishListRepositories
    {
        readonly IDapperContext _context;

        public WishListRepositories(IDapperContext context)
        {

            _context = context;
        }
        public async Task<bool> CreateWishList(AddToCartRequestDTO request, string id)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = $@"
            INSERT INTO WishList (UserId, ProductId, Quantity,CouponId) 
            VALUES ({id}, @ProductId, @Quantity,@CouponId);";


            int rowsAffected = await db.ExecuteAsync(query, request);


            if (rowsAffected == 0)
                return false;

            return true;

        }

        public async Task<List<CartItemDTO>> GetUserWishList(string userId)
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
        WishList c
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

        public async Task<CartItemDTO?> GetWishListById(string userId, int wishlistId)
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
        WishList c
    LEFT JOIN 
        Product p ON c.ProductId = p.Id
    WHERE 
        c.Id = @CartId ;";

            var cartItem = (await db.QueryAsync<CartItemDTO>(cartQuery, new GetCartByIdDTO(userId, wishlistId))).FirstOrDefault();

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

        public async Task<bool> DeleteWishListItem(int wishlistId)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = "DELETE FROM WishList WHERE Id = @wishlistId;";


            int rowsAffected = await db.ExecuteAsync(query, new { WishListId = wishlistId });


            if (rowsAffected == 0)
                return false;

            return true;


        }
        public async Task<bool> DeleteAllUserWishList(string userid)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = "DELETE FROM WishList WHERE UserId = @UserId;";


            int rowsAffected = await db.ExecuteAsync(query, new { UserId = userid });


            if (rowsAffected == 0)
                return false;

            return true;


        }
        public async Task<bool> UpdateWishLists(UpdateCartRequestDTO model)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = "UPDATE WishList SET Quantity = @Quantity WHERE Id = @CartId;";


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

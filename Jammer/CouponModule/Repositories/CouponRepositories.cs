using Dapper;
using Jammer.CouponModule.DTOs;
using Jammer.CouponModule.Models;
using Jammer.CouponModule.Repositories.InterFace;
using Jammer.DBContext;
using Jammer.OrderModule.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Jammer.CouponModule.Repositories
{
    public class CouponRepositories : ICouponRepositories
    {
        readonly IDapperContext _context;

        public CouponRepositories(IDapperContext context)
        {

            _context = context;
        }
        public async Task<bool> CreateCoupon(CouponDTO couponDTO)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"INSERT INTO Coupons (Code, Discount, DiscountType, ExpirationDate)
                         VALUES (@Code, @Discount, @DiscountType, @ExpirationDate);"
            ;

          
              int rowsAffected =   await db.ExecuteAsync(query, couponDTO);

              


            if (rowsAffected == 0)
                return false;

            return true;

        }
        public async Task<List<CouponDTO>> GetAllCoupons()
        {

            using IDbConnection db = _context.CreateConnection();

            string query = @"SELECT * FROM Coupons where ExpirationDate > NOW();";


            return (await db.QueryAsync<CouponDTO>(query)).ToList();
        }
        public async Task<bool> UpdateCoupon(CouponDTO couponDTO)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"UPDATE Coupons SET Code = @Code, Discount = @Discount, DiscountType = @DiscountType, 
         ExpirationDate = @ExpirationDate WHERE Id = @Id;";


            int rowsAffected = await db.ExecuteAsync(query, couponDTO);




            if (rowsAffected == 0)
                return false;

            return true;
        }
        public async Task<bool> ApplyCoupon([FromBody] ApplyCouponRequestDTO request)
        {
            using IDbConnection db = _context.CreateConnection();

            string couponQuery = @"SELECT * FROM Coupons WHERE Id = @Id AND ExpirationDate > NOW();";
            CouponDTO? coupon = await db.QueryFirstOrDefaultAsync<CouponDTO>(couponQuery, new { Id = request.Id });

            if (coupon == null)
                return false; 

           

            if (request.ProductId.HasValue)
            {
                
                string insertCouponProductQuery = @"INSERT INTO couponproduct (CouponId, ProductId) VALUES (@CouponId, @ProductId)";
                await db.ExecuteAsync(insertCouponProductQuery, new { CouponId = coupon.Id, ProductId = request.ProductId.Value });
            }
            else if (request.CategoryId.HasValue)
            {
               
                string insertCouponCategoryQuery = @"INSERT INTO couponcategory (CouponId, CategoryId) VALUES (@CouponId, @CategoryId)";
                await db.ExecuteAsync(insertCouponCategoryQuery, new { CouponId = coupon.Id, CategoryId = request.CategoryId.Value });
            }
            else
            {
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteCoupon(int id)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"DELETE FROM Coupons WHERE Id = @Id;";

           
                int rowsAffected = await db.ExecuteAsync(query, new IntDTOs(id));




                if (rowsAffected == 0)
                return false;

            return true;

        }
            public async Task<CouponDTO?> GetCouponById(int id)
            {
                using IDbConnection db = _context.CreateConnection();

            
                    string query = @"SELECT * FROM Coupons WHERE Id = @id;";
                

               

                    return await db.QueryFirstOrDefaultAsync<CouponDTO?>(query, new { id });
            
            }

        public async Task<List<RandomCouponProductDTO>> GetRandomCouponProductsAsync()
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"
    SELECT 
        p.Id AS ProductId, 
        p.Name AS ProductName, 
        p.Description, 
        p.Price,  p.ProductURL,
        p.StockQuantity,
        c.Id AS CouponId, 
        c.Discount, 
        c.DiscountType, 
        c.ExpirationDate,
        CASE 
            WHEN c.DiscountType = 'PERCENTAGE' THEN p.Price * (1 - c.Discount / 100)
            WHEN c.DiscountType = 'FLAT' THEN 
                CASE 
                    WHEN p.Price - c.Discount >= p.Price * 0.7 THEN p.Price - c.Discount
                    ELSE p.Price * 0.7
                END
        END AS DiscountedPrice
    FROM 
        Product p
    LEFT JOIN 
        CouponProduct cp ON p.Id = cp.ProductId
    LEFT JOIN 
        Coupons c ON cp.CouponId = c.Id
    WHERE 
        c.ExpirationDate > NOW()
    ORDER BY 
        RAND()
    LIMIT 20;";
            var productItems = (await db.QueryAsync<RandomCouponProductDTO>(query)).ToList();

            if (!productItems.Any())
                return productItems;

            var productIds = productItems.Select(ci => ci.ProductId).Distinct().ToList();
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

            foreach (var cartItem in productItems)
            {
                cartItem.ImagePaths = productImagesLookup[cartItem.ProductId].Select(pi => pi.ImagePath).ToList();
            }

            return productItems;
        }
        public async Task<List<RandomCouponProductDTO>> GetProductsByCouponAsync(string couponId)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"
    SELECT 
        p.Id AS ProductId, 
        p.Name AS ProductName, 
        p.Description,  p.ProductURL,
        p.Price, 
        p.StockQuantity,
        c.Code AS CouponCode, 
        c.Discount, 
        c.DiscountType, 
        c.ExpirationDate,
        CASE 
            WHEN c.DiscountType = 'PERCENTAGE' THEN p.Price * (1 - c.Discount / 100)
            WHEN c.DiscountType = 'FLAT' THEN 
                CASE 
                    WHEN p.Price - c.Discount >= p.Price * 0.7 THEN p.Price - c.Discount
                    ELSE p.Price * 0.7
                END
        END AS DiscountedPrice
    FROM 
        Product p
    LEFT JOIN 
        CouponProduct cp ON p.Id = cp.ProductId
    LEFT JOIN 
        Coupons c ON cp.CouponId = c.Id
    WHERE 
        c.id = @id AND c.ExpirationDate > NOW()
    ORDER BY 
        p.Id
    LIMIT 4;";
            var productItems = (await db.QueryAsync<RandomCouponProductDTO>(query, new StringDTOs(couponId))).ToList();

            if (!productItems.Any())
                return productItems;

            var productIds = productItems.Select(ci => ci.ProductId).Distinct().ToList();
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

            foreach (var cartItem in productItems)
            {
                cartItem.ImagePaths = productImagesLookup[cartItem.ProductId].Select(pi => pi.ImagePath).ToList();
            }

            return productItems;
          
        }
        public async Task<RandomCouponProductDTO?> GetProductByProductIdCouponIdAsync(string couponId, string productId)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"
SELECT 
    p.Id AS ProductId, 
    p.Name AS ProductName, 
    p.Description,  
    p.ProductURL,
    p.Price, 
    p.StockQuantity,
    c.Id AS CouponId,  -- Ensure CouponId is included
    c.Code AS CouponCode, 
    c.Discount, 
    c.DiscountType, 
    c.ExpirationDate,
    CASE 
        WHEN c.DiscountType = 'PERCENTAGE' THEN p.Price * (1 - c.Discount / 100)
        WHEN c.DiscountType = 'FLAT' THEN 
            CASE 
                WHEN p.Price - c.Discount >= p.Price * 0.7 THEN p.Price - c.Discount
                ELSE p.Price * 0.7
            END
    END AS DiscountedPrice
FROM 
    Product p
LEFT JOIN 
    CouponProduct cp ON p.Id = cp.ProductId
LEFT JOIN 
    Coupons c ON cp.CouponId = c.Id
WHERE 
    c.Id = @CouponId AND 
    p.Id = @ProductId AND 
    c.ExpirationDate > NOW()
ORDER BY 
    c.ExpirationDate ASC  -- Optional: Order by expiration date, for instance
LIMIT 1;";

            var productItem = await db.QueryFirstOrDefaultAsync<RandomCouponProductDTO>(
                query, new { CouponId = couponId, ProductId = productId });

            if (productItem == null)
                return null;

             string imageQuery = @"
    SELECT 
        pi.ProductId, 
        pi.ImagePath
    FROM 
        ProductImage pi
    WHERE 
        pi.ProductId = @ProductId;"; 
            var productImages = (await db.QueryAsync<ProductImageDTO>(imageQuery, new { ProductId = productItem.ProductId })).ToList();
            productItem.ImagePaths = productImages.Select(pi => pi.ImagePath).ToList();

            return productItem;
        }



        public async Task<List<CouponWithProductsDTO>> GetCouponsWithProductsAsync()
        {
            using IDbConnection db = _context.CreateConnection();

            string couponQuery = @"
    SELECT 
        Id, 
        Code, 
        Discount, 
        DiscountType, 
        ExpirationDate 
    FROM 
        Coupons 
    WHERE 
        ExpirationDate > NOW();";

            var coupons = await db.QueryAsync<CouponDTO>(couponQuery);

            List<CouponWithProductsDTO> result = new();

            foreach (var coupon in coupons)
            {
                string query = @"
        SELECT 
            p.Id AS ProductId, 
            p.Name AS ProductName, 
            p.Description, 
            p.Price,  
            p.ProductURL,
            p.StockQuantity,
            @id AS CouponCode, 
            c.Discount, 
            c.DiscountType, 
            c.ExpirationDate,
            CASE 
                WHEN c.DiscountType = 'PERCENTAGE' THEN p.Price * (1 - c.Discount / 100)
                WHEN c.DiscountType = 'FLAT' THEN 
                    CASE 
                        WHEN p.Price - c.Discount >= p.Price * 0.7 THEN p.Price - c.Discount
                        ELSE p.Price * 0.7
                    END
            END AS DiscountedPrice
        FROM 
            Product p
        LEFT JOIN 
            CouponProduct cp ON p.Id = cp.ProductId
        LEFT JOIN 
            Coupons c ON cp.CouponId = c.Id
        WHERE 
            c.Code = @id AND c.ExpirationDate > NOW()
        ORDER BY 
            p.Id
        LIMIT 4;";

                var productItems = (await db.QueryAsync<RandomCouponProductDTO>(query, new StringDTOs(coupon.Code))).ToList();

                if (!productItems.Any())
                {
                    continue; 
                }
                var productIds = productItems.Select(ci => ci.ProductId).Distinct().ToList();

                string imageQuery = @"
        SELECT 
            pi.ProductId, 
            pi.ImagePath
        FROM 
            ProductImage pi
        WHERE 
            pi.ProductId IN @id;";

                var productImages = (await db.QueryAsync<ProductImageDTO>(imageQuery, new ProductListDTOs( productIds ))).ToList();

                var productImagesLookup = productImages.ToLookup(pi => pi.ProductId);

                foreach (var productItem in productItems)
                {
                    productItem.ImagePaths = productImagesLookup[productItem.ProductId].Select(pi => pi.ImagePath).ToList();
                }

                CouponWithProductsDTO couponWithProducts = new()
                {
                    Coupon = coupon,
                    Products = productItems 
                };

                result.Add(couponWithProducts);
            }

            return result;
        }



        public async Task<List<RandomCouponProductDTO>> GetProductsByDiscountPercentage(int discountPercentage)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"
    SELECT 
        p.Id AS ProductId, 
        p.Name AS ProductName, 
        p.Description, 
        p.Price,  p.ProductURL,
        p.StockQuantity,
        c.Code AS CouponCode, 
        c.Discount, 
        c.DiscountType, 
        c.ExpirationDate,
        CASE 
            WHEN c.DiscountType = 'PERCENTAGE' THEN p.Price * (1 - c.Discount / 100)
            WHEN c.DiscountType = 'FLAT' THEN 
                CASE 
                    WHEN p.Price - c.Discount >= p.Price * 0.7 THEN p.Price - c.Discount
                    ELSE p.Price * 0.7
                END
        END AS DiscountedPrice
    FROM 
        Product p
    LEFT JOIN 
        CouponProduct cp ON p.Id = cp.ProductId
    LEFT JOIN 
        Coupons c ON cp.CouponId = c.Id
    WHERE 
        c.ExpirationDate > NOW()
        AND c.DiscountType = 'PERCENTAGE' 
        AND c.Discount = @id;";
            var productItems = (await db.QueryAsync<RandomCouponProductDTO>(query, new IntDTOs (discountPercentage))).ToList();

            if (!productItems.Any())
                return productItems;

            var productIds = productItems.Select(ci => ci.ProductId).Distinct().ToList();
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

            foreach (var cartItem in productItems)
            {
                cartItem.ImagePaths = productImagesLookup[cartItem.ProductId].Select(pi => pi.ImagePath).ToList();
            }

            return productItems;

           
        }
        public async Task<List<RandomCouponProductDTO>> GetProductsByDiscountRange(decimal minDiscount, decimal maxDiscount)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"
    SELECT 
        p.Id AS ProductId, 
        p.Name AS ProductName, 
        p.Description, 
        p.Price,  p.ProductURL,
        p.StockQuantity,
        c.Code AS CouponCode, 
        c.Discount, 
        c.DiscountType, 
        c.ExpirationDate,
        CASE 
            WHEN c.DiscountType = 'PERCENTAGE' THEN p.Price * (1 - c.Discount / 100)
            WHEN c.DiscountType = 'FLAT' THEN 
                CASE 
                    WHEN p.Price - c.Discount >= p.Price * 0.7 THEN p.Price - c.Discount
                    ELSE p.Price * 0.7
                END
        END AS DiscountedPrice
    FROM 
        Product p
    LEFT JOIN 
        CouponProduct cp ON p.Id = cp.ProductId
    LEFT JOIN 
        Coupons c ON cp.CouponId = c.Id
    WHERE 
        c.ExpirationDate > NOW()
        AND c.DiscountType = 'FLAT' 
        AND (c.Discount BETWEEN @MinDiscount AND @MaxDiscount);";
            var productItems = (await db.QueryAsync<RandomCouponProductDTO>(query, new GetProductsByDiscountRange( minDiscount, maxDiscount ))).ToList();

            if (!productItems.Any())
                return productItems;

            var productIds = productItems.Select(ci => ci.ProductId).Distinct().ToList();
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

            foreach (var cartItem in productItems)
            {
                cartItem.ImagePaths = productImagesLookup[cartItem.ProductId].Select(pi => pi.ImagePath).ToList();
            }

            return productItems;



           }



    }
}

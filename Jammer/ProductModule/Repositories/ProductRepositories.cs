using AutoMapper;
using Dapper;
using Jammer.CartModule.DTOs;
using Jammer.DBContext;
using Jammer.OrderModule.DTOs;
using Jammer.ProductModule.DTOs;
using Jammer.ProductModule.Models;
using Jammer.Utills;
using System.Data;

namespace Jammer.ProductModule.Repositories
{
    public class ProductRepositories: IProductRepository
    {

        readonly IDapperContext _context;

        public ProductRepositories(IDapperContext context )
        {
            
            _context = context;
        }
        public async Task<bool> CreateProductAsync(Product addProductRequest, AddUserProductRequest request, IWebHostEnvironment environment)
        {
            using IDbConnection db = _context.CreateConnection();
            try
            {
                string productQuery = @"
                INSERT INTO Product (Name, Description, Price, StockQuantity, CategoryId, ProductURL) 
    VALUES (@Name, @Description, @Price, @StockQuantity, @CategoryId, @ProductURL);
    SELECT LAST_INSERT_ID();";

                int productId = db.QuerySingle<int>(productQuery, addProductRequest);

                if (request.Images != null && request.Images.Count != 0)
                {
                    string imageQuery = @"
                    INSERT INTO ProductImage (ProductId, ImagePath, CreatedAt) 
                    VALUES (@ProductId, @ImagePath, @CreatedAt);"
                    ;

                    foreach (var image in request.Images)
                    {
                        string imagePath = await FileManage.UploadAsync(image, environment);
                        var productImage = new ProductImage
                        {
                            ProductId = productId,
                            ImagePath = imagePath,
                            CreatedAt = DateTime.UtcNow
                        };

                       await db.ExecuteAsync(imageQuery, productImage);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    //    public async Task<bool> UpdateProductAsync(Product product, UpdateProductRequestDTO request, IWebHostEnvironment environment)
    //    {
    //        using IDbConnection db = _context.CreateConnection();


    //        string productQuery = @"
    //         UPDATE Product 
    //SET Name = @Name, Description = @Description, Price = @Price, 
    //    StockQuantity = @StockQuantity, CategoryId = @CategoryId, ProductURL = @ProductURL
    //WHERE Id = @Id;";

    //        try
    //        {
    //            int rowsAffected = await db.ExecuteAsync(productQuery, product);

    //            if (rowsAffected == 0)
    //                return false;

    //            if (request.Images != null && request.Images.Count != 0)
    //            {
    //                string deleteImagesQuery = @"
    //              DELETE FROM ProductImage 
    //              WHERE ProductId = @ProductId;";

    //                await db.ExecuteAsync(deleteImagesQuery, new { ProductId = request.Id });

    //                string imageQuery = @"
    //     INSERT INTO ProductImage (ProductId, ImagePath, CreatedAt) 
    //     VALUES (@ProductId, @ImagePath, @CreatedAt);";

    //                foreach (var image in request.Images)
    //                {
    //                    string imagePath = await FileManage.UploadAsync(image, environment);
    //                    var productImage = new ProductImage
    //                    {
    //                        ProductId = request.Id,
    //                        ImagePath = imagePath,
    //                        CreatedAt = DateTime.UtcNow
    //                    };

    //                    await db.ExecuteAsync(imageQuery, productImage);
    //                }
    //            }

    //            return true;
    //        }
    //        catch (Exception)
    //        {
    //            return false;
    //        }

    //    }
        public async Task<List<ProductDTO>?> FilterProductsCategory(int parentId, IMapper mapper)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"
            SELECT 
                p.Id, 
                p.Name, 
                p.Description, 
                p.Price, 
                p.StockQuantity, 
                p.CreatedAt, 
                p.ProductURL,
                p.UpdatedAt,
                p.CategoryId as CategoryId,
                c.Name AS CategoryName
            FROM 
                Product p
             LEFT JOIN 
      Categories c ON p.CategoryId = c.Id
            WHERE 
                p.CategoryId = @CategoryId;";
           

            var product = (await db.QueryAsync<ProductDTO>(query, new { CategoryId = parentId })).ToList();

            if (!product.Any())
                return product;

            var productIds = product.Select(ci => ci.Id).Distinct().ToList();

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

            foreach (var cartItem in product)
            {
                cartItem.ImagePath = productImagesLookup[cartItem.Id].Select(pi => pi.ImagePath).ToList();
            }

            return product;

          
            

        }
            public async Task<List<ProductDTO>?> SearchProductsByName(string query, IMapper mapper)
            {
                using IDbConnection db = _context.CreateConnection();


                string sqlQuery = @"
                SELECT 
                    p.Id, 
                    p.Name, 
                    p.Description, 
                    p.Price, 
                    p.StockQuantity, 
                    p.ProductURL,
                    p.CreatedAt, 
                    p.UpdatedAt,
                    p.CategoryId,
                    c.Name AS CategoryName
                FROM 
                    Product p
                LEFT JOIN 
                    Categories c ON p.CategoryId = c.Id

                WHERE 
                    p.Name LIKE @Query;";
            var product = (await db.QueryAsync<ProductDTO>(sqlQuery, new { Query = $"%{query}%" })).ToList();

            if (!product.Any())
                return product;

            var productIds = product.Select(ci => ci.Id).Distinct().ToList();

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

            foreach (var cartItem in product)
            {
                cartItem.ImagePath = productImagesLookup[cartItem.Id].Select(pi => pi.ImagePath).ToList();
            }

            return product;
           
           


            }
         public async Task<List<ProductDTO>?> GetProductsWithPaging(int pageNumber, int pageSize, IMapper mapper)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"
        SELECT 
            p.Id, 
            p.Name, 
            p.Description, 
            p.Price, 
            p.StockQuantity, 
            p.CreatedAt, 
            p.ProductURL,
            p.UpdatedAt,
            p.CategoryId,
            parentProduct.Name AS CategoryName
        FROM 
            Product p
        LEFT JOIN 
            categories parentProduct ON p.CategoryId = parentProduct.Id
        ORDER BY 
            p.CreatedAt DESC
        LIMIT @PageSize OFFSET @Offset;";

            var product = (await db.QueryAsync<ProductDTO>(query, new { Offset = (pageNumber - 1) * pageSize, PageSize = pageSize })).ToList();

            if (!product.Any())
                return product;

            var productIds = product.Select(ci => ci.Id).Distinct().ToList();

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

            foreach (var cartItem in product)
            {
                cartItem.ImagePath = productImagesLookup[cartItem.Id].Select(pi => pi.ImagePath).ToList();
            }

            return product;




        }

        public async Task<ProductDTOWithImageId> GetProductById(int productId, IMapper mapper)
        {
            using IDbConnection db = _context.CreateConnection();
            string query = @"
                SELECT 
                    p.Id, 
                    p.Name, 
                    p.Description, 
                    p.Price, 
                     p.ProductURL,
                    p.StockQuantity, 
                    p.CreatedAt, 
                    p.UpdatedAt,
                    p.CategoryId,
                    c.Name AS CategoryName
                FROM 
                    Product p
  LEFT JOIN 
      Categories c ON p.CategoryId = c.Id
                WHERE 
                    p.Id = @ProductId;";
            var productItem = (await db.QueryAsync<ProductDTOWithImageId>(query, new { ProductId = productId })).FirstOrDefault();

            if (productItem == null)
                return null;
            string imageQuery = @"
SELECT  
    pi.Id, 
    pi.ImagePath
FROM 
    ProductImage pi
WHERE 
    pi.ProductId = @id;";
            var productImages = await db.QueryAsync<ProductImageWithId>(imageQuery, new { id = productItem.Id });

            productItem.ImagePath = productImages.ToList();

            return productItem;



        }
          public async Task<ProductDTO> GetProductByIdBanner(int productId, IMapper mapper)
        {
            using IDbConnection db = _context.CreateConnection();
            string query = @"
                SELECT 
                    p.Id, 
                    p.Name, 
                    p.Description, 
                    p.Price, 
                     p.ProductURL,
                    p.StockQuantity, 
                    p.CreatedAt, 
                    p.UpdatedAt,

                    p.CategoryId,
                    c.Name AS CategoryName
                FROM 
                    Product p
  LEFT JOIN 
      Categories c ON p.CategoryId = c.Ids
                WHERE 
                    p.Id = @ProductId;";
            var productItem = (await db.QueryAsync<ProductDTO>(query, new { ProductId = productId })).FirstOrDefault();

            if (productItem == null)
                return null;
            string imageQuery = @"
SELECT 
    pi.ProductId, 
    pi.ImagePath
FROM 
    ProductImage pi
WHERE 
    pi.ProductId = @id;";
            var productImages = await db.QueryAsync<ProductImageDTO>(imageQuery, new { id = productItem.Id });

            productItem.ImagePath = productImages.Select(pi => pi.ImagePath).ToList();

            return productItem;



        }
        public async Task<ProductDTO> GetProductByURL(string url, IMapper mapper)
        {
            using IDbConnection db = _context.CreateConnection();
            string query = @"
                SELECT 
                    p.Id, 
                    p.Name, 
                    p.Description, 
                    p.Price, 
                     p.ProductURL,
                    p.StockQuantity, 
                    p.CreatedAt, 
                    p.UpdatedAt,

                    p.CategoryId,
                    c.Name AS CategoryName
                FROM 
                    Product p
  LEFT JOIN 
      Categories c ON p.CategoryId = c.Id
                WHERE 

                    p.ProductURL = @URL;";
            var productItem = (await db.QueryAsync<ProductDTO>(query, new { URL = url })).FirstOrDefault();

            if (productItem == null)
                return null;
            string imageQuery = @"
SELECT 
    pi.ProductId, 
    pi.ImagePath
FROM 
    ProductImage pi
WHERE 
    pi.ProductId = @id;";
            var productImages = await db.QueryAsync<ProductImageDTO>(imageQuery, new { id = productItem.Id });

            productItem.ImagePath = productImages.Select(pi => pi.ImagePath).ToList();

            return productItem;


          


        }
        public async Task<bool> UpdateProductStock(UpdateProductStockDTO request)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = "UPDATE Product SET StockQuantity = @NewStockQuantity WHERE Id = @ProductId;";

           
                int rowsAffected = await db.ExecuteAsync(query, request);

                if (rowsAffected == 0)
                    return false;

                return true;
          
        }
        public async Task<bool> UpdateProductAsync(Product product, UpdateProductRequestDTO request, IWebHostEnvironment environment)
        {
            using IDbConnection db = _context.CreateConnection();
            string productQuery = @"
        UPDATE Product 
        SET Name = @Name, Description = @Description, Price = @Price, 
            StockQuantity = @StockQuantity, CategoryId = @CategoryId,  ProductURL = @ProductURL
        WHERE Id = @Id;";

            try
            {
                int rowsAffected = await db.ExecuteAsync(productQuery, product);

                if (rowsAffected == 0)
                    return false;

                if (request.ImageIdsToDelete != null && request.ImageIdsToDelete.Count > 0)
                {
                    string deleteImagesQuery = @"
                DELETE FROM ProductImage 
                WHERE Id IN @ImageIds;";

                    await db.ExecuteAsync(deleteImagesQuery, new { ImageIds = request.ImageIdsToDelete });
                }

                if (request.Images != null && request.Images.Count != 0)
                {
                    string imageQuery = @"
                INSERT INTO ProductImage (ProductId, ImagePath, CreatedAt) 
                VALUES (@ProductId, @ImagePath, @CreatedAt);";

                    foreach (var image in request.Images)
                    {
                        string imagePath = await FileManage.UploadAsync(image, environment);
                        var productImage = new ProductImage
                        {
                            ProductId = product.Id,
                            ImagePath = imagePath,
                            CreatedAt = DateTime.UtcNow
                        };

                        await db.ExecuteAsync(imageQuery, productImage);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<List<ProductIdNameDTO>> GetAllProductIdAndNames()
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"
        SELECT 
            Id, 
            Name 
        FROM 
            Product;";

            var products = (await db.QueryAsync<ProductIdNameDTO>(query)).ToList();
            return products;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            using IDbConnection db = _context.CreateConnection();

            
                string deleteImagesQuery = @"
                    DELETE FROM ProductImage
                    WHERE ProductId = @ProductId;";

               await db.ExecuteAsync(deleteImagesQuery, new { ProductId = id });

                string deleteProductQuery = @"
                    DELETE FROM Product
                    WHERE Id = @Id;";

                int rowsAffected =await db.ExecuteAsync(deleteProductQuery, new { Id = id });

                if (rowsAffected == 0)
                    return false;

                return true;
            
           

        }



        }
}

    using Jammer.DBContext;
    using Jammer.ProductModule.DTOs;
    using Jammer.ProductModule.Repositories.InterFace;
    using Jammer.Utills;
    using System.Data;
    using Dapper;
    using Jammer.ProductModule.Models;
    using Jammer.CartModule.DTOs;

    namespace Jammer.ProductModule.Repositories
    {
        public class ReviewRepository : IReviewRepository
        {
            readonly IDapperContext _context;
            readonly ITokenHelper _tokenHelper;
            readonly IConfiguration _configuration;

            public ReviewRepository(IDapperContext context, ITokenHelper tokenHelper, IConfiguration configuration)
            {
                _context = context;
                _tokenHelper = tokenHelper;
                _configuration = configuration;
            }

            public async Task<bool> AddReviewAsync(AddReviewRequestDTO request,string userId)
            {
                using IDbConnection db = _context.CreateConnection();

                string query = @$"
                INSERT INTO Reviews (ProductId, UserId, Message, Points) 
                VALUES (@ProductId, {userId}, @Message, @Points);"; 

                int rowsAffected = await db.ExecuteAsync(query, request);

                return rowsAffected > 0;
            }

            public async Task<bool> DeleteReview(int id)
            {
                using IDbConnection db = _context.CreateConnection();

                string query = "DELETE FROM Reviews WHERE Id = @Id;";

                int rowsAffected = await db.ExecuteAsync(query, new { Id = id });

                return rowsAffected > 0;
            }

            public async Task<List<Review>> GetReviews(int productId)
            {
                using IDbConnection db = _context.CreateConnection();

                string query = @"
        SELECT 
            r.Id, 
 p.ProductURL,
            r.ProductId, 
            r.UserId,             
            r.CreatedAt, 
            r.Message, 
            r.Points,
            u.fullname,
            u.image	
        FROM 
            Reviews r
        LEFT JOIN 
            User u ON r.UserId = u.Id
     
                LEFT JOIN 
                    Product p ON p.Id = r.ProductId
        WHERE 
            r.ProductId = @ProductId;"; 

                var res = (await db.QueryAsync<Review>(query, new { ProductId = productId })).ToList();
                return res;
            }
        }
    }

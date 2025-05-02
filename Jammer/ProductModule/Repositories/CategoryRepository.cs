using Dapper;
using Jammer.DBContext;
using Jammer.ProductModule.DTOs;
using Jammer.ProductModule.Models;
using Jammer.ProductModule.Repositories.InterFace;
using Jammer.Utills;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data;

namespace Jammer.ProductModule.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        readonly IDapperContext _context;
        readonly ITokenHelper _tokenHelper;
        readonly IConfiguration _configuration;

        public CategoryRepository(IDapperContext context, ITokenHelper tokenHelper, IConfiguration configuration)
        {
            _context = context;
            _tokenHelper = tokenHelper;
            _configuration = configuration;
        }

     public async Task<bool> AddCategory(CategoryRequest request)
{
    using IDbConnection db = _context.CreateConnection();

    string query = @"
        INSERT INTO Categories (Name, ParentId, ImagePath) 
        VALUES (@Name, @ParentId, @ImagePath);";

    var parameters = new CategoryRequest
    {
        Name = request.Name,
        ParentId = request.ParentId == 0 ? (int?)null : request.ParentId,
        ImagePath = request.ImagePath
    };

    int rowsAffected = await db.ExecuteAsync(query, parameters);
    return rowsAffected > 0;
}


        public async Task<bool> UpdateCategory(UpdateCategoryDTO request)
        {
            using IDbConnection db = _context.CreateConnection();
            string query = "UPDATE Categories SET Name = @Name, ParentId = @ParentId, ImagePath = @ImagePath WHERE Id = @Id;";
            var parameters = new UpdateCategoryDTO
            {
                Id = request.Id,
                Name = request.Name,
                ParentId = request.ParentId == 0 ? (int?)null : request.ParentId,
                ImagePath = request.ImagePath
            };
            int rowsAffected = await db.ExecuteAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            using IDbConnection db = _context.CreateConnection();
            string query = "DELETE FROM Categories WHERE Id = @Id;";
            int rowsAffected = await db.ExecuteAsync(query, new { Id = id });
            return rowsAffected > 0;
        }

        public List<CategoryDTO> GetAllCategories()
        {
            using IDbConnection db = _context.CreateConnection();
            string query = @"
                SELECT 
                    c.Id, 
                    c.Name, 
                    c.ParentId, 
                    parent.Name AS ParentName,
                    c.ImagePath 
                FROM 
                    Categories c
                LEFT JOIN 
                    Categories parent ON c.ParentId = parent.Id;";

            return db.Query<CategoryDTO>(query).ToList();
        }

        public async Task<CategoryDTO?> GetCategoryById(int id)
        {
            using IDbConnection db = _context.CreateConnection();
            string query = "SELECT Id, Name, ParentId, ImagePath FROM Categories WHERE Id = @Id;";
            return await db.QueryFirstOrDefaultAsync<CategoryDTO>(query, new { Id = id });
        }
    }
}

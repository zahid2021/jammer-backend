using System.Data;
using Dapper;
using Jammer.DBContext;
using Jammer.CartModule.DTOs;
using Jammer.CartModule.Repositories.InterFace;
using Microsoft.AspNetCore.Mvc;

namespace Jammer.CartModule.Repositories
{
    public class BannerRepository : IBannerRepository
    {
        private readonly IDapperContext _context;

        public BannerRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateBannerAsync(BannerRequestDB request)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = "INSERT INTO Banner (Image,LinkId,Link,CouponId) VALUES (@Image,@LinkId,@Link,@CouponId); SELECT LAST_INSERT_ID();";

            var result = await db.ExecuteAsync(query, request);
            return result; 
        }

        public async Task<IEnumerable<BannerDTO>> GetAllBannersAsync()
        {
            using IDbConnection db = _context.CreateConnection();

            string query = "SELECT * FROM Banner;";

            var banners = await db.QueryAsync<BannerDTO>(query);
            return banners;
        }
        public async Task<bool> DeleteBanner(int Id)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = "DELETE FROM Banner WHERE Id = @id;";


            int rowsAffected = await db.ExecuteAsync(query, new { id= Id });


            if (rowsAffected == 0)
                return false;

            return true;


        } 
       
    }
}

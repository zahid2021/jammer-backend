using Dapper;
using Jammer.DBContext;
using Jammer.OrderModule.DTOs;
using Jammer.OrderModule.Models;
using Jammer.OrderModule.Repositories.InterFace;
using Jammer.ProductModule.DTOs;
using Jammer.Utills;
using System.Data;
namespace Jammer.OrderModule.Repositories
{
    public class AddressRepositories : IAddressRepositories
    {
        readonly IDapperContext _context;
        readonly ITokenHelper _tokenHelper;
        readonly IConfiguration _configuration;

        public AddressRepositories(IDapperContext context, ITokenHelper tokenHelper, IConfiguration configuration)
        {
            _context = context;
            _tokenHelper = tokenHelper;
            _configuration = configuration;
        }

        public async Task<bool> AddAddress(AddAddressRequest request)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @$"
                INSERT INTO Address (UserId, City, Street,PostalCode,Region) 
                VALUES (@UserId,  @City, @Street,@PostalCode,@Region);";

            int rowsAffected = await db.ExecuteAsync(query, request);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAddress(int id)
        {

            using IDbConnection db = _context.CreateConnection();

            string query = "DELETE FROM Address WHERE Id = @Id;";

            int rowsAffected = await db.ExecuteAsync(query, new { Id = id });

            return rowsAffected > 0;
        }
        public async Task<bool> UpdateAddress(Address request)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"
        UPDATE Address 
        SET  City = @City, 
            Street = @Street, 
            PostalCode = @PostalCode, 
            Region = @Region 
        WHERE Id = @Id;";

            int rowsAffected = await db.ExecuteAsync(query, request);

            return rowsAffected > 0;
        }

        public async Task<List<Address>> GetAddress(string userid)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"
        SELECT 
           *
           FROM Address
        WHERE 
            userid = @UserId;";

            var res =( await db.QueryAsync<Address>(query, new { UserId = userid })).ToList();
            return res;
        }
    }
}


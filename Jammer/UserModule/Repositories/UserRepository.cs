using Dapper;
using Jammer.CartModule.DTOs;
using Jammer.CouponModule.Models;
using Jammer.DBContext;
using Jammer.ProductModule.DTOs;
using Jammer.UserModule.DTOs;
using Jammer.UserModule.Models;
using Jammer.UserModule.Repositories.InterFace;
using Jammer.Utills;
using System.Data;

namespace Jammer.UserModule.Repositories
{
    public class UserRepository : IUserRepository
    {

        IDapperContext _context;
        ITokenHelper _tokenHelper;
        IConfiguration _configuration;
        public UserRepository(IDapperContext context, ITokenHelper tokenHelper, IConfiguration configuration)
        {
            _context = context;
            _tokenHelper = tokenHelper;
            _configuration = configuration;
        }

        public async Task<int> Signup(GetUserResponse request)
        {
            using IDbConnection db = _context.CreateConnection();

            int result = await db.ExecuteAsync(@"INSERT INTO user (fullname, passwordhash, email, image, roleid) 
                                         VALUES (@FullName, @PasswordHash, @Email, @Image, @RoleId);",
                                                 request);

            if (result > 0)
            {
                int lastInsertedId = await db.ExecuteScalarAsync<int>("SELECT LAST_INSERT_ID();");
                return lastInsertedId;
            }

            return -1; 
        }
    
    public async Task<int> AddRole(AddRoleRequest request)
        {
            using IDbConnection db = _context.CreateConnection();

           
                int result =await  db.ExecuteAsync(@"INSERT INTO `userrole`( `Name`) VALUES (@Name);
                                          ", request);

            return result;
            
           
        }
        public async Task<UserModel> LoginEmailPass(UserLoginRequest model)
        {
            
                using IDbConnection db = _context.CreateConnection();
                UserModel? user = await db.QuerySingleOrDefaultAsync<UserModel>("SELECT * FROM user WHERE Email = @id ", new StringDTOs(model.Email));

            
                    return user;



        }
        public async Task<List<GetRole>> GetAllRoles()
        {

            using IDbConnection db = _context.CreateConnection();

            string query = @"SELECT * FROM userrole ";


            return (await db.QueryAsync<GetRole>(query)).ToList();
        }
         public async Task<GetRoleName> GetRolesByID(int id)
        {

            using IDbConnection db = _context.CreateConnection();

            string query = @$"SELECT Name FROM userrole where roleid= {id}";


            return await db.QueryFirstOrDefaultAsync<GetRoleName?>(query);
        }
        public async Task<bool> UpdateRole(UpdateRoleRequest request)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"
        UPDATE userrole 
        SET Name = @Name 
        WHERE roleid = @Id;";

            int rowsAffected = await db.ExecuteAsync(query, new { request.Name, request.Id });
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteRole(int id)
        {
            using IDbConnection db = _context.CreateConnection();

            string query = @"
        DELETE FROM userrole 
        WHERE roleid = @Id;";

            int rowsAffected = await db.ExecuteAsync(query, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<List<GetAllUsersResponseWithRoleId>> GetAllUser()
        {
            using IDbConnection db = _context.CreateConnection();


            string query = "SELECT u.id,u.fullname,u.email,u.phonenumber,u.PasswordHash as PasswordHash,u.Image,r.RoleID,r.Name as RoleName FROM user u left JOIN userrole r ON u.RoleId = r.RoleID";

            var res = (await db.QueryAsync<GetAllUsersResponseWithRoleId>(query)).ToList();
            return res;
        }


        public async Task<bool> UpdateUser(GetUpdateRequest request)
        {
            using IDbConnection db = _context.CreateConnection();

            try
            {
                string query = "UPDATE `user` SET `fullname`=@FullName,`passwordhash`=@Passwordhash,`phonenumber`=@PhoneNumber,`image`=@Image WHERE `id`=@Id";
                int result = await db.ExecuteAsync(query, request);

                return result > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }public async Task<bool> UpdateUserByAdmin(GetUpdateByAdminRequest request)
        {
            using IDbConnection db = _context.CreateConnection();

            try
            {
                string query = "UPDATE `user` SET `fullname`=@FullName,`passwordhash`=@Passwordhash,`phonenumber`=@PhoneNumber,`image`=@Image,`roleId`=@RoleId WHERE `id`=@Id";
                int result = await db.ExecuteAsync(query, request);

                return result > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<GetUserResponse?> GetUserById(string id)
        {

            using IDbConnection db = _context.CreateConnection();
            string query = "SELECT * FROM user WHERE id = @id";
            return await db.QuerySingleOrDefaultAsync<GetUserResponse>(query, new StringDTOs(id));
        }
        public async Task<bool> DeleteUserById(string userId)
        {

            using IDbConnection db = _context.CreateConnection();
            var query = "DELETE FROM User WHERE Id = @Id";
            var result = await  db.ExecuteAsync(query, new { Id = userId });
            return result > 0; 
        }
    }
}

using Jammer.ProductModule.DTOs;
using Jammer.ResponseMessage;
using Jammer.UserModule.DTOs;
using Jammer.UserModule.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jammer.UserModule.Repositories.InterFace
{
    public interface IUserRepository
    {
        public Task<int> Signup(GetUserResponse request);
        public Task<bool> UpdateUser(GetUpdateRequest request);
        public Task<bool> UpdateUserByAdmin(GetUpdateByAdminRequest request);
        public Task<List<GetAllUsersResponseWithRoleId>> GetAllUser();
        public Task<UserModel> LoginEmailPass(UserLoginRequest model);
        public Task<GetUserResponse> GetUserById(string id);
        public Task<int> AddRole(AddRoleRequest request);
        public Task<List<GetRole>> GetAllRoles();
        public Task<GetRoleName> GetRolesByID(int id);
        Task<bool> UpdateRole(UpdateRoleRequest request);
        Task<bool> DeleteRole(int id);
    Task<bool> DeleteUserById(string userId);
    }
}

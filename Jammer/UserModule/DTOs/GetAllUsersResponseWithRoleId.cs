namespace Jammer.UserModule.DTOs
{
    public class GetAllUsersResponseWithRoleId
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public string Image { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}

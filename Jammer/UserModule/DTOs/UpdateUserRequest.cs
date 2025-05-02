namespace Jammer.UserModule.DTOs
{
    public class UpdateUserRequest
    {
        public string FullName { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string PhoneNumber { get; set; }
        public IFormFile? Image { get; set; }
    }
        public class UpdateUserByAdminRequest
    {
        public int id { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public IFormFile? Image { get; set; }
        public int RoleId { get; set; }
    }


}

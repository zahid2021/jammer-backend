namespace Jammer.UserModule.DTOs
{
    public class AddUserRequest
    {

        public  string FullName { get; set; }
        public string Email { get; set; }
        public  string Password { get; set; }
        public  string PhoneNumber { get; set; }
        public int RoleId { get; private set; } = 1;
        public IFormFile Image { get; set; }


    }public class AddUserRequestRole
    {

        public  string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public  string Password { get; set; }

        public int RoleId { get; set; }
        public IFormFile Image { get; set; }
         

    }
}

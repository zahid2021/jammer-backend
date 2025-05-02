namespace Jammer.UserModule.DTOs
{
    public class GetUpdateRequest
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Passwordhash { get; set; }
        public string Image { get; set; }
    }public class GetUpdateByAdminRequest
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Passwordhash { get; set; }
        public string Image { get; set; }
        public int RoleId { get; set; }
    }
}

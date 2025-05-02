namespace Jammer.UserModule.DTOs
{
    public class GetRole
    {
        public int RoleId { get; set; }
        public string Name { get; set; }

    }public class GetRoleName
    {
        public string Name { get; set; }

    }
    public class UpdateRoleRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

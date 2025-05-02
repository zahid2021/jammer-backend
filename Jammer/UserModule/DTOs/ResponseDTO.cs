namespace Jammer.UserModule.DTOs
{
    public class ResponseDTO
    {
        public string? Message { get; set; }
        public dynamic? Data { get; set; } = new List<string>();
    }
}

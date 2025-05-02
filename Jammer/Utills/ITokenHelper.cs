namespace Jammer.Utills
{
    public interface ITokenHelper
    {
        public string GenerateToken(string UserId, string Role);
        public bool ValidateToken(string authToken);
        public string GetUserId(string authToken);
    }
}

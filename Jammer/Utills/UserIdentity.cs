using System.Security.Claims;

namespace Jammer.Utills
{
    public static class UserIdentity
    {
        public static int GetUserId(this HttpRequest request)
        {
            return Convert.ToInt32(request.HttpContext.User.Claims.Single(o => o.Type == "UserId").Value);
        }
        public static string GetUser(this HttpRequest request)
        {
            return request.HttpContext.User.Claims.Single(o => o.Type == "UserId").Value;
        }
        public static string GetRole(this HttpRequest request)
        {
            return request.HttpContext.User.Claims.Single(o => o.Type == ClaimTypes.Role).Value;
        }
    }
}

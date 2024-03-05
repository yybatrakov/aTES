namespace AuthorizationServer.Models
{
    public static class AuthUserHelper
    {
        public static string GetUserBeak(string username)
        {
            return username + "Beak";
        }
        public static string GetUserFromBeak(string beak)
        {
            return beak.Replace("Beak", string.Empty);
        }

    }
}

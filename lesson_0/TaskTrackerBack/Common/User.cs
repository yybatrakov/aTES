namespace Common
{
    public class User
    {
        public string UserId { get; set; }
        public string Login { get; set; }
        public string Role { get; set; }

        public User(UserRoles role)
        {
            UserId = role.ToString();
            Login = role.ToString();
            Role = role.ToString();
        }
    }
}
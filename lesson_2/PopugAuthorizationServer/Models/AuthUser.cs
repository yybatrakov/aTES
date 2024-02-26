namespace AuthorizationServer.Models
{
    using System.ComponentModel.DataAnnotations;

    public class AuthUser
    {
        [Required]
        public string Beak { get; set; }
    }
}

using Common;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        public AuthController()
        {
        }

        [HttpGet("Login")]
        public IResult Login(string login)
        {
            var user = DataConstants.Users.Where(x => x.Login == login).SingleOrDefault();
            if (user == null)
                throw new Exception("unknown user");

            return Results.Ok(user);
        }
    }
}
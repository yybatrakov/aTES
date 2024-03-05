using AuthorizationServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthorizationServer.Controllers
{

    public class UsersController : Controller
    {
        public UsersLogic UserLogic { get; }

        public UsersController(UsersLogic userLogic)
        {
            UserLogic = userLogic;
        }
        [HttpGet("users/get")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin")]
        public async Task<IActionResult> Get()
        {
            return Ok(await UserLogic.GetUsers());
        }
        [HttpPost("users/add")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin")]
        public async Task<IActionResult> Add(AuthUser user, string role)
        {
            return Ok(await UserLogic.AddUser(user.Beak, role));
        }

        [HttpPut("users/update")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin")]
        public async Task<IActionResult> Update(AuthUser user, string role)
        {
            return Ok(await UserLogic.UpdateUser(user.Beak, role));
        }

        [HttpDelete("users/delete")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin")]
        public async Task<IActionResult> Delete(AuthUser user)
        {
            return Ok(await UserLogic.DeleteUser(user.Beak));
        }
    }
}

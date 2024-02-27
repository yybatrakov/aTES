using AuthorizationServer.Models;
using AuthorizationServer.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServer.Controllers
{

    public class UserController : Controller
    {
        private readonly DataContext dataContext;
        private readonly UserManager<IdentityUser> userManager;

        public UserController(DataContext dataContext, UserManager<IdentityUser> userManager)
        {
            this.dataContext = dataContext;
            this.userManager = userManager;
        }
        [HttpGet("users/get")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin")]
        public async Task<IActionResult> Get()
        {
            return Ok(dataContext.Users.ToListAsync());
        }
        [HttpPost("users/add")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin")]
        public async Task<IActionResult> Add(AuthUser user, string role)
        {
            var result = await userManager.CreateAsync(new IdentityUser
            {
                UserName = AuthUserHelper.GetUserFromBeak(user.Beak)
            }, user.Beak);
            var identity = await dataContext.Users.Where(u => u.UserName == AuthUserHelper.GetUserFromBeak(user.Beak)).FirstOrDefaultAsync();

            result = await userManager.AddToRoleAsync(identity, role);

            Kafka.Produce("Users", identity.UserName, (new UserMessage() { 
                MessageId = Guid.NewGuid().ToString(),
                UserName = identity.UserName,
                UserRole = role,
                Operation = CudOperation.Add,
                OpertionDate = DateTime.Now
            }).ToJson());

            return Ok(result);
        }

        [HttpDelete("users/delete")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin")]
        public async Task<IActionResult> Delete(AuthUser user)
        {
            var identity = await dataContext.Users.Where(u => u.UserName == AuthUserHelper.GetUserFromBeak(user.Beak)).FirstOrDefaultAsync();
            var result = await userManager.DeleteAsync(identity);
            Kafka.Produce("Users", identity.UserName, (new UserMessage()
            {
                MessageId = Guid.NewGuid().ToString(),
                UserName = identity.UserName,
                Operation = CudOperation.Delete,
                OpertionDate = DateTime.Now
            }).ToJson());

            return Ok(result);
        }
    }
}

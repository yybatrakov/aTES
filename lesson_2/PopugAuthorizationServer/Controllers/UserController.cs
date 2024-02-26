using AuthorizationServer.Models;
using AuthorizationServer.Persistence;
using Confluent.Kafka;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mxm.Kafka;
using PopugCommon;
using PopugCommon.KafkaMessages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
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
        [AcceptVerbs("Get")]
        [HttpGet]
        [Route("users/get")]
        public async Task<IActionResult> Get()
        {
            return Ok(dataContext.Users.ToListAsync());
        }

        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("users/add")]
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

        [AcceptVerbs("Delete")]
        [HttpDelete]
        [Route("users/delete")]
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

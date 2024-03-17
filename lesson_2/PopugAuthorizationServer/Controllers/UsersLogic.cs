using AuthorizationServer.Models;
using AuthorizationServer.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServer.Controllers
{
    public class UsersLogic
    {
        private readonly DataContext dataContext;
        private readonly UserManager<IdentityUser> userManager;

        public UsersLogic(DataContext dataContext, UserManager<IdentityUser> userManager)
        {
            this.dataContext = dataContext;
            this.userManager = userManager;
        }
        public async Task<List<IdentityUser>> GetUsers()
        {
            return await dataContext.Users.ToListAsync();
        }

        public async Task<IdentityUser> AddUser(string userBeak, string role)
        {
            var result = await userManager.CreateAsync(new IdentityUser
            {
                UserName = AuthUserHelper.GetUserFromBeak(userBeak)
            }, userBeak);
            var identity = await dataContext.Users.Where(u => u.UserName == AuthUserHelper.GetUserFromBeak(userBeak)).FirstOrDefaultAsync();

            result = await userManager.AddToRoleAsync(identity, role);

            await Kafka.Produce(KafkaTopics.UsersStream, identity.Id, new PopugMessage(new UserStreamEvent()
            {
                UserId = identity.Id,
                PublicId = identity.Id,
                UserName = identity.UserName,
                UserRole = role
            }, KafkaMessages.Users.Stream.Created, "v1"));

            return identity;
        }
        public async Task<IdentityUser> UpdateUser(string userBeak, string role)
        {
            var identity = await dataContext.Users.Where(u => u.UserName == AuthUserHelper.GetUserFromBeak(userBeak)).FirstOrDefaultAsync();
            var userRoles = await userManager.GetRolesAsync(identity);
            await userManager.RemoveFromRolesAsync(identity, userRoles);
            var result = await userManager.AddToRoleAsync(identity, role);

            await Kafka.Produce(KafkaTopics.UsersStream, identity.Id, new PopugMessage(new UserStreamEvent()
            {
                UserId = identity.Id,
                PublicId = identity.Id,
                UserName = identity.UserName,
                UserRole = role
            }, KafkaMessages.Users.Stream.Updated, "v1"));

            return identity;
        }
        public async Task<IdentityUser> DeleteUser(string userBeak)
        {
            var identity = await dataContext.Users.Where(u => u.UserName == AuthUserHelper.GetUserFromBeak(userBeak)).FirstOrDefaultAsync();
            var result = await userManager.DeleteAsync(identity);
            await Kafka.Produce(KafkaTopics.UsersStream, identity.Id, new PopugMessage(new UserStreamEvent()
            {
                UserId = identity.Id,
                PublicId = identity.Id,
                UserName = identity.UserName,
                UserRole = string.Empty
            }, KafkaMessages.Users.Stream.Deleted, "v1"));
            return identity;
        }
    }
}

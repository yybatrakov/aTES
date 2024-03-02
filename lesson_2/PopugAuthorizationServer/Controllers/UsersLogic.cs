﻿using AuthorizationServer.Models;
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

            Kafka.Produce(KafkaTopics.UsersStream, identity.Id, new StreamMessage<User>(new User()
            {
                UserId = identity.Id,
                UserName = identity.UserName,
                UserRole = role
            }, Operation.Add).ToJson());

            return identity;
        }
        public async Task<IdentityUser> UpdateUser(string userBeak, string role)
        {
            var identity = await dataContext.Users.Where(u => u.UserName == AuthUserHelper.GetUserFromBeak(userBeak)).FirstOrDefaultAsync();
            var userRoles = await userManager.GetRolesAsync(identity);
            await userManager.RemoveFromRolesAsync(identity, userRoles);
            var result = await userManager.AddToRoleAsync(identity, role);

            Kafka.Produce(KafkaTopics.UsersStream, identity.Id, new StreamMessage<User>(new User()
            {
                UserId = identity.Id,
                UserName = identity.UserName,
                UserRole = role
            }, Operation.Update).ToJson());

            return identity;
        }
        public async Task<IdentityUser> DeleteUser(string userBeak)
        {
            var identity = await dataContext.Users.Where(u => u.UserName == AuthUserHelper.GetUserFromBeak(userBeak)).FirstOrDefaultAsync();
            var result = await userManager.DeleteAsync(identity);
            Kafka.Produce(KafkaTopics.UsersStream, identity.Id, new StreamMessage<User>(new User()
            {
                UserId = identity.Id,
                UserName = identity.UserName
            }, Operation.Delete).ToJson());

            return identity;
        }
    }
}
using Confluent.Kafka;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using System.Linq;
using System.Threading.Tasks;
using static PopugTaskTracker.DataContext;

namespace PopugTaskTracker.Logic
{
    public class UsersLogic
    {
        private readonly DataContext dataContext;
        
        public UsersLogic(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public async Task<User> Get(string userId)
        {
            return await dataContext.Users.Where(u => u.UserId == userId).FirstOrDefaultAsync();
        }
        public async Task<User> AddOrUpdate(User user)
        {
            var userDb = await Get(user.UserId);
            if (userDb != null)
            {
                dataContext.Users.Update(user);
                await dataContext.SaveChangesAsync();
            }
            else
            {
                await dataContext.Users.AddAsync(user);
                await dataContext.SaveChangesAsync();
            }
            
            return await Get(user.UserId);
        }
        public async Task<User> Delete(User user)
        {
            dataContext.Users.Remove(user);
            return await Get(user.UserId);
        }

    }
}

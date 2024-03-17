using Microsoft.EntityFrameworkCore;
using PopugCommon.KafkaMessages;
using System.Linq;
using System.Threading.Tasks;

namespace PopugTaskTracker.Logic
{
    public class UsersLogic
    {
        private readonly DataContext dataContext;
        
        public UsersLogic(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public async Task<UserDb> Get(string publicUserId)
        {
            return await dataContext.Users.Where(u => u.PublicId == publicUserId).FirstOrDefaultAsync();
        }
        public async Task<UserDb> AddOrUpdate(UserStreamEvent user)
        {
            var userDb = await Get(user.UserId);
            if (userDb != null)
            {
                userDb.UserName = user.UserName;
                userDb.UserRole = user.UserRole;
                dataContext.Users.Update(userDb);
                await dataContext.SaveChangesAsync();
            }
            else
            {
                userDb = new UserDb() { PublicId = user.PublicId, UserName = user.UserName, UserRole = user.UserName };
                await dataContext.Users.AddAsync(userDb);
                await dataContext.SaveChangesAsync();
            }
            
            return await Get(userDb.PublicId);
        }
        public async Task Delete(UserStreamEvent user)
        {
            var userDb = await Get(user.UserId);
            if (userDb != null) {
                dataContext.Users.Remove(userDb);
                dataContext.SaveChanges();
            }
        }

    }
}

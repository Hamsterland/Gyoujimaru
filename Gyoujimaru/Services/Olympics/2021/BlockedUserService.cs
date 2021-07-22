using System.Linq;
using System.Threading.Tasks;
using Gyoujimaru.Data;
using Microsoft.EntityFrameworkCore;

namespace Gyoujimaru.Services.Olympics._2021
{
    public class BlockedUserService
    {
        private readonly GyoujimaruContext _gyoujimaruContext;

        public BlockedUserService(GyoujimaruContext gyoujimaruContext)
        {
            _gyoujimaruContext = gyoujimaruContext;
        }

        public async Task Block(ulong userId)
        {
            var user = await _gyoujimaruContext
                .BlockedUsers
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user is not null)
            {
                return;
            }

            user = new BlockedUser
            {
                UserId = userId
            };

            _gyoujimaruContext.Add(user);
            await _gyoujimaruContext.SaveChangesAsync();
        }

        public async Task Unblock(ulong userId)
        {
            var user = await _gyoujimaruContext
                .BlockedUsers
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user is null)
            {
                return;
            }
            
            _gyoujimaruContext.Remove(user);
            await _gyoujimaruContext.SaveChangesAsync();
        }
        
        public async Task<bool> IsUserBlocked(ulong userId)
        {
            var users = await _gyoujimaruContext
                .BlockedUsers
                .AsNoTracking()
                .ToListAsync();

            return users.Any(x => x.UserId == userId);
        }
    }
}
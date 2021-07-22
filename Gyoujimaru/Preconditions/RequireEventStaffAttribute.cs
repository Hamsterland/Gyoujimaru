using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;

namespace Gyoujimaru.Preconditions
{
    public class RequireEventStaffAttribute : PreconditionAttribute
    {
        private const ulong _myAnimeListGuildId = 301123999000166400;
        private const ulong _eventStaffRoleId = 555604217289768984;

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var socketCommandContext = context as SocketCommandContext;
            var guild = socketCommandContext.Client.GetGuild(_myAnimeListGuildId);
            var user = guild.GetUser(context.User.Id);

            return Task.FromResult(user.Roles.Any(x => x.Id == _eventStaffRoleId) 
                ? PreconditionResult.FromSuccess() 
                : PreconditionResult.FromError("Only event staff can use this command."));
        }
    }
}
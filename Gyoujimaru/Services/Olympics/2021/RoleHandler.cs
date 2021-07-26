using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gyoujimaru.Notifications;
using MediatR;

namespace Gyoujimaru.Services.Olympics
{
    public class RoleHandler : 
        INotificationHandler<ReactionAddedNotification>, 
        INotificationHandler<ReactionRemovedNotification>
    {
        private readonly DiscordSocketClient _client;

        public RoleHandler(DiscordSocketClient client)
        {
            _client = client;
        }

        private const ulong _championsRoleId = 869006491317829652;
        private const ulong _myAnimeListId = 301123999000166400;
        private const ulong _messageId = 869024180438446080;
        
        public async Task Handle(ReactionAddedNotification notification, CancellationToken cancellationToken)
        {
            var message = notification.Message;

            if (message.Id != _messageId)
            {
                return;
            }

            var reaction = notification.Reaction;
            
            var user = _client
                .GetGuild(_myAnimeListId)
                .GetUser(reaction.UserId);

            // If the user doesn't have the Champions role, add it.
            if (!user.Roles.Select(x => x.Id).Contains(_championsRoleId))
            {
                await user.AddRoleAsync(_championsRoleId);
                await user.SendMessageAsync("You now have the **Champions** role.");
            }
        }

        public async Task Handle(ReactionRemovedNotification notification, CancellationToken cancellationToken)
        {
            var message = notification.Message;

            if (message.Id != _messageId)
            {
                return;
            }

            var reaction = notification.Reaction;

            var user = _client
                .GetGuild(_myAnimeListId)
                .GetUser(reaction.UserId);

            // If the user does have the Champions role, remove it.
            if (user.Roles.Select(x => x.Id).Contains(_championsRoleId))
            {
                await user.RemoveRoleAsync(_championsRoleId);
                await user.SendMessageAsync("You no longer have the **Champions** role.");
            }
        }
    }
}
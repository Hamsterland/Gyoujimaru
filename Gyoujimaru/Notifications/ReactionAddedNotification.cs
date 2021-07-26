using Discord;
using Discord.WebSocket;
using MediatR;

namespace Gyoujimaru.Notifications
{
    public class ReactionAddedNotification : INotification
    {
        public Cacheable<IUserMessage, ulong> Message { get; }
        public ISocketMessageChannel Channel { get; }
        public SocketReaction Reaction { get; }

        public ReactionAddedNotification(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            Message = message;
            Channel = channel;
            Reaction = reaction;
        }
    }
}
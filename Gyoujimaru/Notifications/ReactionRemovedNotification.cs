using Discord;
using Discord.WebSocket;
using MediatR;

namespace Gyoujimaru.Notifications
{
    public class ReactionRemovedNotification : INotification
    {
        public Cacheable<IUserMessage, ulong> Message { get; }
        public ISocketMessageChannel Channel { get; }
        public SocketReaction Reaction { get; }

        public ReactionRemovedNotification(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            Message = message;
            Channel = channel;
            Reaction = reaction;
        }
    }
}
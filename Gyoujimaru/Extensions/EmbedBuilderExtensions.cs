using Discord;

namespace Gyoujimaru.CustomExtensions
{
    public static class EmbedBuilderExtensions
    {
        public static EmbedBuilder WithUserAsAuthor(this EmbedBuilder embedBuilder, IUser user, string titleOverride = null)
        {
            return embedBuilder
                .WithAuthor(author => author
                    .WithIconUrl(user.GetAvatarUrl())
                    .WithName(titleOverride ?? $"{user} ({user.Id})"));
        }
    }
}
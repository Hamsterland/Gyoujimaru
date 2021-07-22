using System;
using System.Threading.Tasks;
using Discord.Commands;
using Gyoujimaru.Services.Olympics._2021;
using Microsoft.Extensions.DependencyInjection;

namespace Gyoujimaru.Preconditions
{
    public class BlockControlAttribute : PreconditionAttribute
    {
        private readonly BlockOptions _blockOption;

        public BlockControlAttribute(BlockOptions blockOption)
        {
            _blockOption = blockOption;
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var blockedUserService = services.GetRequiredService<BlockedUserService>();
            var isBlocked = await blockedUserService.IsUserBlocked(context.User.Id);

            return _blockOption switch
            {
                BlockOptions.RequireUnblocked when isBlocked => PreconditionResult.FromError("You have been blocked from using event commands."),
                BlockOptions.RequireUnblocked => PreconditionResult.FromSuccess(),
                BlockOptions.RequireBlocked when isBlocked => PreconditionResult.FromSuccess(),
                BlockOptions.RequireBlocked => PreconditionResult.FromError("Only blocked users can use this command. You are currently unblocked."),
                _ => PreconditionResult.FromError("I encountered an unexpected error. You have been blocked from using this command for safety.")
            };
        }
    }

    public enum BlockOptions
    {
        RequireUnblocked,
        RequireBlocked
    }
}
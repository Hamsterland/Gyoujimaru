using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Gyoujimaru.MyAnimeList.Characters;
using Gyoujimaru.Preconditions;
using Gyoujimaru.Services.Olympics._2021;
using Gyoujimaru.Services.Olympics._2021.Tracker;
using Interactivity;

namespace Gyoujimaru.Modules
{
    [Name("Staff Module")]
    [Summary("Commands for Event Staff.")]
    [RequireEventStaff]
    [RequireContext(ContextType.Guild | ContextType.DM)]
    public class StaffModule : ModuleBase<SocketCommandContext>
    {
        private readonly BlockedUserService _blockedUserService;
        private readonly CharacterService _characterService;
        private readonly CharacterClient _characterClient;
        private readonly InteractivityService _interactivityService;
        private readonly TrackerService _trackerService;

        public StaffModule(
            BlockedUserService blockedUserService,
            CharacterService characterService, 
            CharacterClient characterClient, 
            InteractivityService interactivityService, 
            TrackerService trackerService)
        {
            _blockedUserService = blockedUserService;
            _characterService = characterService;
            _characterClient = characterClient;
            _interactivityService = interactivityService;
            _trackerService = trackerService;
        }

        [Command("advance")]
        [Summary("Advances all non-eliminated villains to the next stage.")]
        public async Task Advance()
        {
            await _characterService.ModifyStage(CharacterService.StageOptions.Advance);
            await ReplyAsync("Advanced non-eliminated characters to the next stage.");
        }

        [Command("retreat")]
        [Summary("Retreat all non-eliminated villains to the previous stage.")]
        public async Task Retreat()
        {
            await _characterService.ModifyStage(CharacterService.StageOptions.Retreat);
            await ReplyAsync("Retreated non-eliminated characters to the previous stage.");
        }

        [Command("advancesheet", RunMode = RunMode.Async)]
        [Summary("Writes all the next-stage characters to the sheet")]
        public async Task AdvanceInSheet()
        {
            var embed = new EmbedBuilder()
                .WithColor(Color.Red)
                .WithTitle("ATTENTION")
                .WithDescription("ONLY USE THIS COMMAND ONCE YOU'RE SURE THAT ALL ELIMINATED CHARACTERS HAVE BEEN ELIMINATED USING ~eliminate AND YOU ADVANCE NON-ELIMINATED CHARACTERS' STAGES USING ~advance")
                .WithFooter("React to this message with anything to confirm.")
                .Build();

            var msg = await ReplyAsync(embed: embed);
            var reaction = await _interactivityService.NextReactionAsync(x => x.MessageId == msg.Id && x.UserId == Context.User.Id);

            if (reaction.IsSuccess)
            {
                var submissions = await _characterService.GetAllSubmissions();
                
                submissions = submissions
                    .Where(x => !x.IsEliminated)
                    .ToList();

                foreach (var submission in submissions)
                {
                    await _trackerService.AddCharacter(submission, submissions.IndexOf(submission));
                }
            }
        }

        [Command("block")]
        [Summary("Blocks a user from using Event commands.")]
        public async Task Block(IUser user)
        {
            await _blockedUserService.Block(user.Id);
            await ReplyAsync($"{user.Mention} ({user.Id}) has been blocked from using Event commands. If {user.Username} has already submitted a villain, their submission will still count.");
        }

        [Command("unblock")]
        [Summary("Unblocks a user from using Event commands.")]
        public async Task Unblock(IUser user)
        {
            await _blockedUserService.Unblock(user.Id);
            await ReplyAsync($"{user.Mention} ({user.Id}) has been unblocked from using Event commands.");
        }

        [Command("eliminate")]
        [Summary("Eliminates a user's villain.")]
        public async Task Eliminate(IUser eliminated, IUser eliminator)
        {
            var submission = await _characterService.GetSubmission(eliminated.Id);
            var eliminatorSubmission = await _characterService.GetSubmission(eliminator.Id);
            
            if (!await _characterService.TryEliminate(eliminated.Id, eliminator.Id))
            {
                await ReplyAsync("Either the user being eliminated or the eliminator doesn't have a claimed villain. Therefore, no changes were made.");
                return;
            }
            
            var character = await _characterClient.GetCharacterFromId(submission.CharacterId);
            await ReplyAsync($"{eliminated.Mention}'s ({eliminated.Id}) villain, {character.RomanisedName}, was eliminated by {eliminator.Mention}'s ({eliminator.Id}) villain, {eliminatorSubmission.Name}.");
        }

        [Command("remove"), Alias("delete")]
        [Summary("Removes a user's claim to a character.")]
        public async Task Delete(IUser user)
        {
            var submission = await _characterService.GetSubmission(user.Id);
            
            if (!await _characterService.TryDeleteSubmission(user.Id))
            {
                await ReplyAsync($"{user.Mention} does not have a claimed villain to remove.");
                return;
            }

            var character = await _characterClient.GetCharacterFromId(submission.CharacterId);
            await ReplyAsync($"Removed {user.Mention}'s ({user.Id}) claim to {character.RomanisedName}");
        }

        [Command("xinfo")]
        [Summary("Shows extended information about a user's villain. If no user is supplied, information about your own villain is shown.")]
        public async Task Info(IUser user = null)
        {
            user ??= Context.User;
            
            var submission = await _characterService.GetSubmission(user.Id);

            if (submission is null)
            {
                var message = user.Id == Context.User.Id 
                    ? "You don't have a claimed villain." 
                    : $"{user.Mention} ({user.Id} doesn't have a claimed villain.";

                await ReplyAsync(message);
                return;
            }

            var infoEmbed = new CharacterInfoEmbedBuilder(submission, Context.Client, true)
                .WithAllOptions()
                .WithFooter("If you're not event staff, you shouldn't be seeing this.")
                .Build();
                
            await ReplyAsync(embed: infoEmbed);
        }
    }
}
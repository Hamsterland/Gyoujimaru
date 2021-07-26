using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Gyoujimaru.CustomExtensions;
using Gyoujimaru.MyAnimeList.Characters;

namespace Gyoujimaru.Modules
{
    public partial class CharacterModule
    {
        private readonly IEmote _up = new Emoji("👍");
        private readonly IEmote _down = new Emoji("👎");
        private readonly Regex _characterUrlRegex = new(@"(https://)?(myanimelist.net/character/)(?<id>[1-9]+)/([a-zA-z]+)?");
        
        [Command("submit", RunMode = RunMode.Async)]
        [Summary("Submit a villain to claim.")]
        public async Task Submit(string input)
        {
            var submissions = await _characterService.GetAllSubmissions();

            if (submissions.Count > 256)
            {
                await ReplyAsync("There is no space left in this event, sorry!");
                return;
            }
            
            var matches = _characterUrlRegex.Matches(input);
            
            if (matches.Count == 0)
            {
                await HandleInvalidUrl(input);
                return;
            }

            if (await UserOwnsCharacter())
            {
                return;
            }

            var characterToClaim = await _characterClient.GetCharacterFromUrl(input);

            if (await AnotherUserOwnsCharacter(characterToClaim))
            {
                return;
            }

            var match = matches.FirstOrDefault();
            var reaction = await GetNextReaction(match.Value);

            if (reaction is null)
            {
                await HandleNoReaction(characterToClaim);
                return;
            }
            
            if (IsConfirmReaction(reaction))
            {
                await HandleConfirmedReaction(characterToClaim);
                return;
            }

            if (IsDenyReaction(reaction))
            {
                await HandleDeniedReaction(characterToClaim);
                return;
            }

            await HandleInvalidReaction();
        }

        private async Task HandleNoReaction(Character characterToClaim)
        {
            var embed = new EmbedBuilder()
                .WithUserAsAuthor(Context.User)
                .WithDescription(
                    $"20 seconds has passed, [{characterToClaim.RomanisedName}]({characterToClaim.Url}) has not been claimed as your villain due to timeout.")
                .Build();

            await ReplyAsync(embed: embed);
        }

        private async Task<bool> UserOwnsCharacter()
        {
            var existingCharacterSubmission = await _gyoujimaruContext
                .CharacterSubmissions
                .FirstOrDefaultAsync(x => x.ClaimantId == Context.User.Id);

            if (existingCharacterSubmission is not null)
            {
                var character = await _characterClient.GetCharacterFromId(existingCharacterSubmission.CharacterId);
            
                await ReplyAsync(embed: new EmbedBuilder()
                    .WithUserAsAuthor(Context.User)
                    .WithDescription($"Sorry, but you've already claimed [{character.RomanisedName}]({character.Url}) so you can't claim another villain.")
                    .Build());
                
                return true;
            }

            return false;
        }

        private async Task<bool> AnotherUserOwnsCharacter(Character characterToClaim)
        {
            var alreadyClaimedCharacterSubmission = await _gyoujimaruContext
                .CharacterSubmissions
                .FirstOrDefaultAsync(x => x.CharacterId == characterToClaim.Id);

            if (alreadyClaimedCharacterSubmission is not null)
            {
                await ReplyAsync(embed: new EmbedBuilder()
                    .WithUserAsAuthor(Context.User)
                    .WithDescription($"Sorry, but someone else has already claimed [{characterToClaim.RomanisedName}]({characterToClaim.Url}).")
                    .Build());
                
                return true;
            }

            return false;
        }

        private async Task<SocketReaction> GetNextReaction(string url)
        {
            var character = await _characterClient.GetCharacterFromUrl(url);

            var embed = new EmbedBuilder()
                .WithUserAsAuthor(Context.User)
                .WithDescription($"Are you sure you want to submit [{character.RomanisedName}]({character.Url}) as your chosen villain? This can't be changed, and make sure your chosen character is actually a villain.")
                .WithFooter($"You have {_interactivityService.DefaultTimeout.TotalSeconds} seconds to choose")
                .WithThumbnailUrl(character.ImageUrl)
                .Build();

            var selection = await Context.Channel.SendMessageAsync(embed: embed);
            await selection.AddReactionsAsync(new[] { _up, _down });
            var reaction = await _interactivityService.NextReactionAsync(x => x.UserId == Context.User.Id);
            return reaction.Value;
        }

        private bool IsConfirmReaction(IReaction reaction)
        {
            return reaction.Emote.Equals(_up);
        }
        
        private bool IsDenyReaction(IReaction reaction)
        {
            return reaction.Emote.Equals(_down);
        }

        private async Task HandleConfirmedReaction(Character character)
        {
            var submission = await _characterService.CreateSubmission(Context.User.Id, character.Id, character.RomanisedName, character.Url, character.ImageUrl);

            await _trackerService.AddCharacter(submission);
            
            var embed = new EmbedBuilder()
                .WithUserAsAuthor(Context.User)
                .WithDescription($"You've chosen [{character.RomanisedName}]({character.Url}) as your villain.")
                .Build();

            await ReplyAsync(embed: embed);
        }

        private async Task HandleDeniedReaction(Character character)
        {
            var embed = new EmbedBuilder()
                .WithUserAsAuthor(Context.User)
                .WithDescription($"You've chosen not to select [{character.RomanisedName}]({character.Url}) as your villain.")
                .Build();

            await ReplyAsync(embed: embed);
        }

        private async Task HandleInvalidReaction() 
        {
            var embed = new EmbedBuilder()
                .WithUserAsAuthor(Context.User)
                .WithDescription($"I'm sorry, but the reaction you provided was invalid. Please ~submit again and react with {_up} to confirm or {_down} to deny.")
                .Build();

            await ReplyAsync(embed: embed);
        }

        private async Task HandleInvalidUrl(string invalidInpit)
        {
            await ReplyAsync(embed: new EmbedBuilder()
                .WithUserAsAuthor(Context.User)
                .WithTitle("Invalid Input")
                .WithDescription($"{invalidInpit} is not a valid MyAnimeList character Url.")
                .AddField("Example Urls", "\nhttps://myanimelist.net/character/161472/Nino_Nakano\nhttps://myanimelist.net/character/467/Gorou_Honda\nhttps://myanimelist.net/character/164471/")
                .AddField("Example Usage", "~submit https://myanimelist.net/character/161472/Nino_Nakano\n~submit https://myanimelist.net/character/467/Gorou_Honda")
                .Build());
        }
        
        private async Task HandleAlreadyOwnedCharacter(Character toClaimCharacter)
        {
            await ReplyAsync(embed: new EmbedBuilder()
                .WithUserAsAuthor(Context.User)
                .WithDescription($"Sorry, but someone else has already claimed [{toClaimCharacter.RomanisedName}]({toClaimCharacter.Url}).")
                .Build());
        }
    }
}
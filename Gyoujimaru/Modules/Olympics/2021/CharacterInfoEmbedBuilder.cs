using System;
using System.Text;
using Discord;
using Discord.WebSocket;
using Gyoujimaru.Services.Olympics._2021;

namespace Gyoujimaru.Modules
{
    public class CharacterInfoEmbedBuilder : EmbedBuilder
    {
        private readonly CharacterSubmission _characterSubmission;
        private readonly DiscordSocketClient _client;
        private readonly bool _showClaims;

        public CharacterInfoEmbedBuilder(CharacterSubmission characterSubmission, DiscordSocketClient client, bool showClaims = false)
        {
            _characterSubmission = characterSubmission;
            _client = client;
            _showClaims = showClaims;
        }

        public CharacterInfoEmbedBuilder WithCharacterName()
        {
            AddField("Character Name", $"[{_characterSubmission.Name}]({_characterSubmission.Url})");
            return this;
        }
        
        public CharacterInfoEmbedBuilder WithCharacterId()
        {
            AddField("Character Id", $"{_characterSubmission.CharacterId}");
            return this;
        }

        public CharacterInfoEmbedBuilder WithCharacterImage()
        {
            WithThumbnailUrl(_characterSubmission.ImageUrl);
            return this;
        }
        
        public CharacterInfoEmbedBuilder WithStage()
        {
            AddField("Event Stage", $"{_characterSubmission.Stage}");
            return this;
        }

        public CharacterInfoEmbedBuilder WithEliminated()
        {
            AddField("Eliminated?", _characterSubmission.IsEliminated);
            return this;
        }

        public CharacterInfoEmbedBuilder WithEliminatedBy()
        {
            var eliminatedBy = _characterSubmission.EliminatedBy;

            if (eliminatedBy is null)
            {
                AddField("Eliminated By", "No One");
                return this;
            }

            if (_showClaims)
            {
                var claimaint = _client.GetUser(eliminatedBy.ClaimantId);
                AddField("Eliminated By", $"[{eliminatedBy.Name}]({eliminatedBy.Url}) claimed by {claimaint}");
            }
            else
            {
                AddField("Eliminated By", $"[{eliminatedBy.Name}]({eliminatedBy.Url})");
            }
            
            return this;
        }
        
        public CharacterInfoEmbedBuilder WithAllOptions()
        {
            WithCharacterName();
            WithCharacterId();
            WithCharacterImage();
            WithStage();
            WithEliminated();
            WithEliminatedBy();
            return this;
        }
    }
}
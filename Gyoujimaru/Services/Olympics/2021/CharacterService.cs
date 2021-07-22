using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gyoujimaru.Data;
using Microsoft.EntityFrameworkCore;

namespace Gyoujimaru.Services.Olympics._2021
{
    public class CharacterService
    {
        private readonly GyoujimaruContext _gyoujimaruContext;

        public CharacterService(GyoujimaruContext gyoujimaruContext)
        {
            _gyoujimaruContext = gyoujimaruContext;
        }

        public async Task<CharacterSubmission> CreateSubmission(ulong claimantId, int characterId, string name, string url, string imageUrl)
        {
            var characterSubmission = new CharacterSubmission
            {
                ClaimantId = claimantId,
                CharacterId = characterId,
                Name = name,
                Url = url,
                ImageUrl = imageUrl
            };

            _gyoujimaruContext.Add(characterSubmission);
            await _gyoujimaruContext.SaveChangesAsync();
            return characterSubmission;
        }

        public async Task<CharacterSubmission> GetSubmission(ulong userId)
        {
            return await _gyoujimaruContext
                .CharacterSubmissions
                .FirstOrDefaultAsync(x => x.ClaimantId == userId);
        }

        public async Task<List<CharacterSubmission>> GetAllSubmissions()
        {
            return await _gyoujimaruContext
                .CharacterSubmissions
                .ToListAsync();
        }

        public async Task<bool> TryDeleteSubmission(ulong userId)
        {
            var character = await _gyoujimaruContext
                .CharacterSubmissions
                .FirstOrDefaultAsync(x => x.ClaimantId == userId);

            if (character is null)
            {
                return false;
            }

            _gyoujimaruContext.Remove(character);
            await _gyoujimaruContext.SaveChangesAsync();
            return true;
        }

        public async Task ModifyStage(StageOptions stageOptions)
        {
            var submissions = _gyoujimaruContext
                .CharacterSubmissions
                .Where(x => !x.IsEliminated);

            foreach (var submission in submissions)
            {
                switch (stageOptions)
                {
                    case StageOptions.Advance:
                        submission.Stage++;
                        break;
                    case StageOptions.Retreat:
                        submission.Stage--;
                        break;
                    default:
                        throw new InvalidOperationException($"An invalid {nameof(stageOptions)} was provided. No stages were modified.");
                }
            }

            await _gyoujimaruContext.SaveChangesAsync();
        }

        public async Task<bool> TryEliminate(ulong userId, ulong eliminatorId)
        {
            var submission = await GetSubmission(userId);

            if (submission is null)
            {
                return false;
            }

            var eliminatorSubmission = await GetSubmission(eliminatorId);

            if (eliminatorSubmission is null)
            {
                return false;
            }

            submission.IsEliminated = true;
            submission.EliminatedBy = eliminatorSubmission;
            await _gyoujimaruContext.SaveChangesAsync();
            return true;
        }

        public enum StageOptions
        {
            Advance,
            Retreat
        }
    }
}
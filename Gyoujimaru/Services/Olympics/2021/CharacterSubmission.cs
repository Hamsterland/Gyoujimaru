using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gyoujimaru.Services.Olympics._2021
{
    public class CharacterSubmission
    {
        public int Id { get; set; }
        public ulong ClaimantId { get; set; }
        public int CharacterId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public bool IsEliminated { get; set; }
        public int Stage { get; set; } = 1;
        public CharacterSubmission EliminatedBy { get; set; }
    }

    public class CharacterSubmissionConfiguration : IEntityTypeConfiguration<CharacterSubmission>
    {
        public void Configure(EntityTypeBuilder<CharacterSubmission> builder)
        {
            builder.HasKey(x => x.Id);

            builder
                .Property(x => x.ClaimantId)
                .HasConversion<long>();

            builder
                .HasIndex(x => x.CharacterId)
                .IsUnique();

            builder
                .HasIndex(x => x.ClaimantId)
                .IsUnique();
        }
    }
}
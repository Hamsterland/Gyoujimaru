using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gyoujimaru.Services.Olympics._2021
{
    public class BlockedUser
    {
        public ulong UserId { get; set; }
    }

    public class BlockedUserConfiguration : IEntityTypeConfiguration<BlockedUser>
    {
        public void Configure(EntityTypeBuilder<BlockedUser> builder)
        {
            builder.HasKey(x => x.UserId);

            builder
                .Property(x => x.UserId)
                .HasConversion<long>();
        }
    }
}
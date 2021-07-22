using Gyoujimaru.Services.Olympics._2021;
using Microsoft.EntityFrameworkCore;

namespace Gyoujimaru.Data
{
    public class GyoujimaruContext : DbContext
    {
        public DbSet<CharacterSubmission> CharacterSubmissions { get; set; }

        public DbSet<BlockedUser> BlockedUsers { get; set; }
        
        public GyoujimaruContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);
        }
    }
}
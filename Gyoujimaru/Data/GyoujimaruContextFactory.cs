using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Gyoujimaru.Data
{
    public class GyoujimaruContextFactory : IDesignTimeDbContextFactory<GyoujimaruContext>
    {
        public GyoujimaruContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<GyoujimaruContext>()
                .Build();
            
            var optionsBuilder = new DbContextOptionsBuilder()
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .UseNpgsql(configuration["Postgres:Connection"]);
            
            return new GyoujimaruContext(optionsBuilder.Options);
        }
    }
}
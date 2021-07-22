using Microsoft.Extensions.Configuration;

namespace Gyoujimaru.CustomExtensions
{
    public static class ConfigurationExtensions
    {
        public static string GetToken(this IConfiguration configuration)
        {
            return configuration["Discord:Token"];
        }
        
        public static string GetPrefix(this IConfiguration configuration)
        {
            return configuration["Discord:Prefix"];
        }
        
        public static string GetConnectionString(this IConfiguration configuration)
        {
            return configuration["Postgres:Connection"];
        }
    }
}
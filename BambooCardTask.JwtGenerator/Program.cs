using Microsoft.Extensions.Configuration;

namespace BambooCardTask.JwtGenerator
{
    class Program
    {
        static void Main(string[] args)
        {

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) // Set the base path to the application's directory
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Ensure the file is loaded correctly
                .Build();

            // Generate JWT tokens for different roles
            Console.WriteLine("JWT Token for User:");
            Console.WriteLine(JwtTokenGenerator.GenerateToken("User", "user@bamboocard.ae", configuration));

            Console.WriteLine("JWT Token for Admin:");
            Console.WriteLine(JwtTokenGenerator.GenerateToken("Admin", "admin@bamboocard.ae", configuration));

            Console.ReadLine();
        }
    }
}
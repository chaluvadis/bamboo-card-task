using BambooCardTask.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BambooCardTask.Test.Configuration;

public class JwtConfigurationTests
{
    [Fact]
    public void ConfigureJwtAuthentication_ShouldThrowException_WhenJwtKeyIsMissing()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => services.ConfigureJwtAuthentication(configuration));
        Assert.Equal("JWT secret key is not configured.", exception.Message);
    }

    [Fact]
    public void ConfigureJwtAuthentication_ShouldAddJwtBearerAuthentication_WhenConfigurationIsValid()
    {
        // Arrange
        var services = new ServiceCollection();
        var initialData = new Dictionary<string, string>
            {
                { "Jwt:Key", "b8e2f3c4d5a6e7f8g9h0i1j2k3l4m5n6o7p8q9r0s1t2u3v4w5x6y7z8a9b0c1d2" },
                { "Jwt:Issuer", "bamboocard.ae" },
                { "Jwt:Audience", "bamboocard.ae" }
            };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(initialData)
            .Build();

        // Act
        services.ConfigureJwtAuthentication(configuration);
        var serviceProvider = services.BuildServiceProvider();
        var authenticationSchemeProvider = serviceProvider.GetService<IAuthenticationSchemeProvider>();

        // Assert
        Assert.NotNull(authenticationSchemeProvider);
        Assert.Contains(authenticationSchemeProvider.GetAllSchemesAsync().Result, scheme => scheme.Name == JwtBearerDefaults.AuthenticationScheme);
    }
}

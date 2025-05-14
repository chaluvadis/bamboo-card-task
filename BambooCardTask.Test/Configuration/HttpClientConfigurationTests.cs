using BambooCardTask.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BambooCardTask.Test.Configuration
{
    public class HttpClientConfigurationTests
    {
        [Fact]
        public void ConfigureHttpClient_ShouldThrowException_WhenProviderNameIsMissing()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => services.ConfigureHttpClient(configuration));
            Assert.Equal("Provider name is not configured in appsettings.json", exception.Message);
        }

        [Fact]
        public void ConfigureHttpClient_ShouldThrowException_WhenProviderSectionIsMissing()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> { { "Provider:Name", "TestProvider" } })
                .Build();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => services.ConfigureHttpClient(configuration));
            Assert.Equal("Configuration section for provider 'TestProvider' is not found.", exception.Message);
        }

        [Fact]
        public void ConfigureHttpClient_ShouldThrowException_WhenExchangeRateApiUrlIsMissing()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Provider:Name", "TestProvider" },
                    { "TestProvider:BaseCurrency", "USD" }
                })
                .Build();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => services.ConfigureHttpClient(configuration));
            Assert.Equal("ExchangeRateApiUrl is not configured for provider 'TestProvider' in appsettings.json", exception.Message);
        }

        [Fact]
        public void ConfigureHttpClient_ShouldAddHttpClient_WhenConfigurationIsValid()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Provider:Name", "TestProvider" },
                    { "TestProvider:BaseCurrency", "USD" },
                    { "TestProvider:ExchangeRateApiUrl", "https://api.testprovider.com" }
                })
                .Build();

            // Act
            services.ConfigureHttpClient(configuration);
            var serviceProvider = services.BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            // Assert
            Assert.NotNull(httpClientFactory);
        }
    }
}

using BambooCardTask.Services;

namespace BambooCardTask.Test.Services;

public class CorrelationIdServiceTests
{
    [Fact]
    public void CorrelationId_ShouldBeGenerated_WhenServiceIsInstantiated()
    {
        // Arrange & Act
        var service = new CorrelationIdService();

        // Assert
        Assert.False(string.IsNullOrEmpty(service.CorrelationId));
    }

    [Fact]
    public void CorrelationId_ShouldBeUnique_ForEachInstance()
    {
        // Arrange & Act
        var service1 = new CorrelationIdService();
        var service2 = new CorrelationIdService();

        // Assert
        Assert.NotEqual(service1.CorrelationId, service2.CorrelationId);
    }
}

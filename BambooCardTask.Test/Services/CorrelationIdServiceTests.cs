using BambooCardTask.Services;

namespace BambooCardTask.Test.Services;

public class CorrelationIdServiceTests
{
    [Fact]
    public void CorrelationId_ShouldBeGenerated_WhenServiceIsInstantiated()
    {
        // Arrange
        var service = new CorrelationIdService();

        // Act
        var correlationId = service.CorrelationId;

        // Assert
        Assert.False(string.IsNullOrEmpty(correlationId));
    }

    [Fact]
    public void CorrelationId_ShouldBeUnique_ForEachInstance()
    {
        // Arrange
        var service1 = new CorrelationIdService();
        var service2 = new CorrelationIdService();

        // Act
        var correlationId1 = service1.CorrelationId;
        var correlationId2 = service2.CorrelationId;

        // Assert
        Assert.NotEqual(correlationId1, correlationId2);
    }
}

using BambooCardTask.Interfaces;
using BambooCardTask.Services;
using Xunit;

namespace BambooCardTask.Test.Interfaces
{
    public class ICorrelationIdServiceTests
    {
        [Fact]
        public void CorrelationId_ShouldBeGenerated_WhenServiceIsInstantiated()
        {
            // Arrange
            ICorrelationIdService service = new CorrelationIdService();

            // Act
            var correlationId = service.CorrelationId;

            // Assert
            Assert.False(string.IsNullOrEmpty(correlationId));
        }

        [Fact]
        public void CorrelationId_ShouldBeUnique_ForEachInstance()
        {
            // Arrange
            ICorrelationIdService service1 = new CorrelationIdService();
            ICorrelationIdService service2 = new CorrelationIdService();

            // Act
            var correlationId1 = service1.CorrelationId;
            var correlationId2 = service2.CorrelationId;

            // Assert
            Assert.NotEqual(correlationId1, correlationId2);
        }
    }
}

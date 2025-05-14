namespace BambooCardTask.Services;

public class CorrelationIdService : ICorrelationIdService
{
    public string CorrelationId { get; private set; }

    public CorrelationIdService() => CorrelationId = Guid.NewGuid().ToString("N");
}

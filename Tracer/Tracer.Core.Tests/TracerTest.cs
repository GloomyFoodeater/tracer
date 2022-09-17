namespace Tracer.Core.Tests;

public class TracerTest
{
    [Fact]
    public void EmptyResult()
    {
        // Arrange
        ITracer tracer = new Tracer();

        // Act
        TraceResult result = tracer.GetTraceResult();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Threads);
        Assert.Equal(0, result.Threads.Count);
    }
}
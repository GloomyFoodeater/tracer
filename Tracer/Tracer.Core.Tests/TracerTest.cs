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
    
    private static bool TimeToInt(MethodInfo info, out int actualTime) => int.TryParse(info.Time[..^2], out actualTime);

    [Fact]
    public void OneMethod()
    {
        // Arrange
        ITracer tracer = new Tracer();

        // Act
        WorkloadImitator imitator = new(tracer);
        imitator.M0();
        TraceResult result = tracer.GetTraceResult();
        
        // Assert
        Assert.NotNull(result);
        
        Assert.NotNull(result.Threads);
        Assert.Equal(1, result.Threads.Count);
        
        Assert.NotNull(result.Threads[0].Methods);
        Assert.Equal(1, result.Threads[0].Methods.Count);
        
        // Get actual and estimated time in int format.
        MethodInfo info = result.Threads[0].Methods[0];
        bool isParsable = TimeToInt(info, out int actualTime);
        int estimatedTime = WorkloadImitator.GetEstimatedTime(nameof(WorkloadImitator.M0)) + 100;
        
        Assert.True(isParsable);
        Assert.InRange(actualTime, actualTime, estimatedTime);
        Assert.Equal(nameof(WorkloadImitator.M0),info.Name);
        Assert.Equal(nameof(WorkloadImitator), info.Class);
        Assert.Null(info.Methods);
    }
}
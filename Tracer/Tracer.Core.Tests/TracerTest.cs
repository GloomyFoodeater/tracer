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

        AssertM0(result.Threads[0].Methods[0]);

        AssertTime(result.Threads[0]);
    }

    [Fact]
    public void ManyMethods()
    {
        // Arrange
        ITracer tracer = new Tracer();
        WorkloadImitator imitator = new(tracer);

        // Act
        imitator.M1();
        imitator.M0();
        imitator.M1();
        TraceResult result = tracer.GetTraceResult();

        // Assert
        Assert.NotNull(result);

        Assert.NotNull(result.Threads);
        Assert.Equal(1, result.Threads.Count);

        Assert.NotNull(result.Threads[0].Methods);
        Assert.Equal(3, result.Threads[0].Methods.Count);

        AssertM1(result.Threads[0].Methods[0]);
        AssertM0(result.Threads[0].Methods[1]);
        AssertM1(result.Threads[0].Methods[2]);

        AssertTime(result.Threads[0]);
    }

    [Fact]
    public void ManyThreads()
    {
        // Arrange
        ITracer tracer = new Tracer();
        WorkloadImitator imitator = new WorkloadImitator(tracer);

        // Act
        Thread t1 = new Thread(imitator.M0);
        t1.Start();
        Thread t2 = new Thread(imitator.M1);
        t2.Start();
        t1.Join();
        t2.Join();
        imitator.M2();

        TraceResult result = tracer.GetTraceResult();

        // Assert
        Assert.NotNull(result);

        Assert.NotNull(result.Threads);
        Assert.Equal(4, result.Threads.Count);

        // First thread
        Assert.NotNull(result.Threads[0]);
        Assert.Equal(1, result.Threads[0].Methods.Count);
        AssertM0(result.Threads[0].Methods[0]);
        AssertTime(result.Threads[0]);

        // Second thread
        Assert.NotNull(result.Threads[1]);
        Assert.Equal(1, result.Threads[1].Methods.Count);
        AssertM1(result.Threads[1].Methods[0]);
        AssertTime(result.Threads[1]);

        // Third thread
        Assert.NotNull(result.Threads[2]);
        Assert.Equal(1, result.Threads[2].Methods.Count);
        AssertM2(result.Threads[2].Methods[0]);
        AssertTime(result.Threads[2]);
        
        // Fourth thread
        Assert.NotNull(result.Threads[3]);
        Assert.Equal(1, result.Threads[3].Methods.Count);
        AssertM0(result.Threads[3].Methods[0]);
        AssertTime(result.Threads[3]);
    }

    private bool TimeToInt(MethodInfo info, out int actualTime) => int.TryParse(info.Time[..^2], out actualTime);

    private void AssertM0(MethodInfo method)
    {
        Assert.NotNull(method);

        // Get actual and estimated time in int format.
        bool isParsable = TimeToInt(method, out int actualTime);
        int estimatedTime = WorkloadImitator.GetEstimatedTime(nameof(WorkloadImitator.M0)) + 100;

        Assert.True(isParsable);
        Assert.InRange(actualTime, actualTime, estimatedTime);
        Assert.Equal(nameof(WorkloadImitator.M0), method.Name);
        Assert.Equal(nameof(WorkloadImitator), method.Class);
        Assert.Null(method.Methods);
    }

    private void AssertM1(MethodInfo method)
    {
        Assert.NotNull(method);

        // Get actual and estimated time in int format.
        bool isParsable = TimeToInt(method, out int actualTime);
        int estimatedTime = WorkloadImitator.GetEstimatedTime(nameof(WorkloadImitator.M1)) + 100;

        Assert.True(isParsable);
        Assert.InRange(actualTime, actualTime, estimatedTime);
        Assert.Equal(nameof(WorkloadImitator.M1), method.Name);
        Assert.Equal(nameof(WorkloadImitator), method.Class);
        Assert.Equal(2, method.Methods.Count);
        AssertM0(method.Methods[0]);
        AssertM0(method.Methods[1]);
    }

    private void AssertM2(MethodInfo method)
    {
        Assert.NotNull(method);

        // Get actual and estimated time in int format.
        bool isParsable = TimeToInt(method, out int actualTime);
        int estimatedTime = WorkloadImitator.GetEstimatedTime(nameof(WorkloadImitator.M2)) + 100;

        Assert.True(isParsable);
        Assert.InRange(actualTime, actualTime, estimatedTime);
        Assert.Equal(nameof(WorkloadImitator.M2), method.Name);
        Assert.Equal(nameof(WorkloadImitator), method.Class);
        Assert.Equal(1, method.Methods.Count);
        AssertM0(method.Methods[0]);
    }

    private void AssertTime(ThreadInfo thread)
    {
        int sum = 0;
        foreach (MethodInfo info in thread.Methods)
        {
            TimeToInt(info, out int delta);
            sum += delta;
        }

        Assert.Equal(thread.Time, sum + "ms");
    }
}
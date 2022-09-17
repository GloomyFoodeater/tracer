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
        Assert.Equal(1, result.Threads.Count);
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
        Assert.Equal(1, result.Threads.Count);
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
        Assert.Equal(4, result.Threads.Count);
        
        // Each thread has only 1 method => loop
        List<string> actualNames = new();
        foreach (var thread in result.Threads)
        {
            Assert.Equal(1, thread.Methods.Count);
            string name = AssertMethodByName(thread.Methods[0]);
            actualNames.Add(name);
            AssertTime(thread);
        }

        string[] expectedNames =
        {
            nameof(WorkloadImitator.M0), 
            nameof(WorkloadImitator.M1), 
            nameof(WorkloadImitator.M2),
            nameof(WorkloadImitator.M0) // WorkloadImitator.M2 invokes M0 in new thread
        };
        
        Assert.True(actualNames.SequenceEqual(expectedNames));
    }

    private bool TimeToInt(MethodInfo info, out int actualTime) => int.TryParse(info.Time[..^2], out actualTime);

    private void AssertM0(MethodInfo method)
    {
        // Get actual and estimated time in int format.
        bool isParsable = TimeToInt(method, out int actualTime);
        int estimatedTime = WorkloadImitator.GetEstimatedTime(nameof(WorkloadImitator.M0)) + 100;

        Assert.True(isParsable);
        Assert.InRange(actualTime, actualTime, estimatedTime);
        Assert.Equal(nameof(WorkloadImitator.M0), method.Name);
        Assert.Equal(nameof(WorkloadImitator), method.Class);
        Assert.Null(method.Methods);
    }

    private string AssertMethodByName(MethodInfo method)
    {
        switch (method.Name)
        {
            case nameof(WorkloadImitator.M0):
                AssertM0(method);
                break;
            case nameof(WorkloadImitator.M1):
                AssertM1(method);
                break;
            case nameof(WorkloadImitator.M2):
                AssertM2(method);
                break;
            default:
                throw new ArgumentException($"Asserting incorrect method {method.Class}.{method.Name}");
        }

        return method.Name;
    }

    private void AssertM1(MethodInfo method)
    {
        // Get actual and estimated time in int format.
        bool isParsable = TimeToInt(method, out int actualTime);
        int estimatedTime = WorkloadImitator.GetEstimatedTime(nameof(WorkloadImitator.M1)) + 100;

        Assert.True(isParsable);
        Assert.InRange(actualTime, actualTime, estimatedTime);
        Assert.Equal(nameof(WorkloadImitator.M1), method.Name);
        Assert.Equal(nameof(WorkloadImitator), method.Class);
        Assert.Equal(2, method.Methods!.Count);
        AssertM0(method.Methods[0]);
        AssertM0(method.Methods[1]);
    }

    private void AssertM2(MethodInfo method)
    {
        // Get actual and estimated time in int format.
        bool isParsable = TimeToInt(method, out int actualTime);
        int estimatedTime = WorkloadImitator.GetEstimatedTime(nameof(WorkloadImitator.M2)) + 100;

        Assert.True(isParsable);
        Assert.InRange(actualTime, actualTime, estimatedTime);
        Assert.Equal(nameof(WorkloadImitator.M2), method.Name);
        Assert.Equal(nameof(WorkloadImitator), method.Class);
        Assert.Equal(1, method.Methods!.Count);
        AssertM0(method.Methods[0]);
    }

    private void AssertTime(ThreadInfo thread)
    {
        int sum = 0;
        foreach (var info in thread.Methods)
        {
            TimeToInt(info, out int delta);
            sum += delta;
        }

        Assert.Equal(thread.Time, sum + "ms");
    }
}
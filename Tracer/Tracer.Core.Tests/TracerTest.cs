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
        Thread t2 = new Thread(imitator.M1);

        t1.Start();
        t2.Start();
        imitator.M2();
        imitator.M0();
        imitator.M1();
        t1.Join();
        t2.Join();

        TraceResult result = tracer.GetTraceResult();

        // Array with thread ids to assert functions specific for each thread.
        int[] tids = { t1.ManagedThreadId, t2.ManagedThreadId, Thread.CurrentThread.ManagedThreadId };

        // Assert
        Assert.Equal(4, result.Threads.Count);
        foreach (var threadInfo in result.Threads)
        {
            
            int expectedLength;
            
            // Checks specific for each thread.
            if (threadInfo.Id == tids[0])
            {
                expectedLength = 1;
                AssertM0(threadInfo.Methods[0]);
            }
            else if (threadInfo.Id == tids[1])
            {
                expectedLength = 1;
                AssertM1(threadInfo.Methods[0]);
            }
            else if (threadInfo.Id == tids[2])
            {
                expectedLength = 3;
                AssertM2(threadInfo.Methods[0]);
                AssertM0(threadInfo.Methods[1]);
                AssertM1(threadInfo.Methods[2]);
            }
            else
            {
                expectedLength = 1;
                AssertM0(threadInfo.Methods[0]);
            }
            
            // Check for every thread.
            AssertTime(threadInfo);
            Assert.Equal(expectedLength, threadInfo.Methods.Count);
        }
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
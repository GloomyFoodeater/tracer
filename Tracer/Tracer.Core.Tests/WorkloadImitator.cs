namespace Tracer.Core.Tests;

internal class WorkloadImitator
{
    private readonly ITracer _tracer;

    public WorkloadImitator(ITracer tracer)
    {
        _tracer = tracer;
    }

    public void M0()
    {
        _tracer.StartTrace();
        Thread.Sleep(100);
        _tracer.StopTrace();
    }

    public void M1()
    {
        _tracer.StartTrace();
        Thread.Sleep(50);
        M0();
        M0();
        _tracer.StopTrace();
    }

    public void M2()
    {
        _tracer.StartTrace();
        Thread thread = new Thread(M0);
        thread.Start();
        M0();
        _tracer.StopTrace();
        thread.Join();
    }
    
    public static int GetEstimatedTime(string method)
    {
        return method switch
        {
            nameof(M0) => 100,
            nameof(M1) => 250,
            nameof(M2) => 150,
            _ => -1
        };
    }
}
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

    public static int GetEstimatedTime(string method)
    {
        switch (method)
        {
            case nameof(M0):
                return 100;
            default:
                return 0;
        }
    }
}
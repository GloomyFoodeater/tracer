using Tracer.Core;

namespace Tracer.Example;

class Worker
{
    private ITracer _tracer;

    public Worker(ITracer tracer)
    {
        _tracer = tracer;
    }

    public void SimpleMethod()
    {
        _tracer.StartTrace();
        Thread.Sleep(100);
        _tracer.StopTrace();
    }

    public void OuterMethod(int counter)
    {
        _tracer.StartTrace();
        if (counter > 0)
        {
            OuterMethod(counter - 1);
        }
        else
        {
            Thread.Sleep(25);
            SimpleMethod();
        }

        _tracer.StopTrace();
    }

    public void MultiThreadWithRootMethod()
    {
        _tracer.StartTrace();
        Thread.Sleep(100);
        Thread thread = new Thread(() => OuterMethod(4));
        thread.Start();
        thread.Join();
        _tracer.StopTrace();
    }

    public void StartAdjacentMethods()
    {
        SimpleMethod();
        OuterMethod(2);
    }
}
using Tracer.Core;
using Tracer.Example;
using Tracer.Serialization;

// Imitate work in 5 threads.
void ImitateWork(ITracer tracer)
{
    Worker worker = new(tracer);
    Thread[] threads =
    {
        new(worker.SimpleMethod),
        new(() => worker.OuterMethod(2)),
        new(worker.StartAdjacentMethods)
    };
    foreach (var thread in threads)
    {
        thread.Start();
    }

    worker.SimpleMethod();
    worker.OuterMethod(1);
    worker.StartAdjacentMethods();

    worker.MultiThreadWithRootMethod();

    foreach (var thread in threads)
    {
        thread.Join();
    }
}

// Trace execution flow.
ITracer tracer = new Tracer.Core.Tracer();
ImitateWork(tracer);
var result = tracer.GetTraceResult();

// Serialize tracer result.
Console.WriteLine("Start of serialization.");
var serializer = new CombinedSerializer("Plugins\\Serializers");
serializer.Write(result, "result");
Console.WriteLine("End of serialization.");
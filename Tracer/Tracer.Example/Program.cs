using Tracer.Core;
using Tracer.Example;
using static Tracer.Serialization.SerializerLoader;

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

// Serializes tracer result.
Console.WriteLine("Start of serialization.");
var serializers = LoadSerializers("Plugins\\Serializers");
foreach (var serializer in serializers)
{
    Console.WriteLine($"Serializing trace result in {Directory.GetCurrentDirectory()}\\result.{serializer.Format}...");
    using var to = new FileStream($"result.{serializer.Format}", FileMode.Create);
    serializer.Serialize(result, to);
}

Console.WriteLine("End of serialization.");
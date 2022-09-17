using Tracer.Core;
using static Tracer.Serialization.SerializerLoader;

// Use tracer in 6 threads to show an example.
void ImitateWork(ITracer tracer)
{
    tracer.StartTrace();
    WorkloadImitator imitator = new(tracer);
    Thread[] threads =
    {
        new(imitator.M0),
        new(imitator.M1),
        new(imitator.M2)
    };

    foreach (var thread in threads) thread.Start();

    imitator.M0();
    imitator.M1();
    imitator.M2();

    foreach (var thread in threads) thread.Join();

    tracer.StopTrace();
}

// Trace execution flow.
ITracer tracer = new Tracer.Core.Tracer();
ImitateWork(tracer);
var result = tracer.GetTraceResult();

// Serializes tracer result.
Console.WriteLine("Start serialization.");
var serializers = LoadSerializers("Plugins\\Serializers");
foreach (var serializer in serializers)
{
    Console.WriteLine($"Serializing trace result in {Directory.GetCurrentDirectory()}\\result.{serializer.Format}...");
    using var to = new FileStream($"result.{serializer.Format}", FileMode.Create);
    serializer.Serialize(result, to);
}

Console.WriteLine("End serialization.");
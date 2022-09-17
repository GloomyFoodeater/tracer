using System.Reflection;
using Tracer.Core;
using Tracer.Core.Tests;
using Tracer.Serialization.Abstractions;

// Load all serializers from plugins in given directory.
List<ITraceResultSerializer> LoadSerializers(string directory = "Plugins\\Serializers")
{
    List<ITraceResultSerializer> serializers = new();
    var plugins = Directory.GetFiles(directory, "*.dll");
    foreach (string plugin in plugins)
    {
        try
        {
            Assembly assembly = Assembly.LoadFrom(plugin);

            // Get all types, which implement interface.
            var types = assembly.GetTypes();
            foreach (Type type in types)
            {
                // Check if type implements interface.
                var interfaces = type.GetInterfaces();
                if (type.FullName != null && interfaces.Contains(typeof(ITraceResultSerializer)))
                {
                    var serializer = (assembly.CreateInstance(type.FullName) as ITraceResultSerializer) !;
                    serializers.Add(serializer);
                }
            }
        }
        catch
        {
            // Ignored.
        }
    }

    return serializers;
}

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

    foreach (var thread in threads)
    {
        thread.Start();
    }

    imitator.M0();
    imitator.M1();
    imitator.M2();

    foreach (var thread in threads)
    {
        thread.Join();
    }

    tracer.StopTrace();
}

// Trace execution flow.
ITracer tracer = new Tracer.Core.Tracer();
ImitateWork(tracer);
TraceResult result = tracer.GetTraceResult();

// Serializes tracer result.
Console.WriteLine("Start serialization.");
List<ITraceResultSerializer> serializers = LoadSerializers();
foreach (var serializer in serializers)
{
    Console.WriteLine($"Serializing trace result in {Directory.GetCurrentDirectory()}\\result.{serializer.Format}...");
    using var to = new FileStream($"result.{serializer.Format}", FileMode.Create);
    serializer.Serialize(result, to);
}
Console.WriteLine("End serialization.");
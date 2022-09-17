using System.Reflection;
using Tracer.Core;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization;

public class CombinedSerializer
{
    private readonly List<ITraceResultSerializer> _serializers;

    public CombinedSerializer() => _serializers = new List<ITraceResultSerializer>();

    public CombinedSerializer(string directory): this() => LoadPlugins(directory);

    public void LoadPlugins(string directory)
    {
        string[] plugins = Directory.GetFiles(directory, "*.dll");
        foreach (var plugin in plugins)
        {
            try
            {
                var assembly = Assembly.LoadFrom(plugin);

                // Get all types, which implement interface.
                Type[] types = assembly.GetTypes();
                foreach (var type in types)
                {
                    // Check if type implements interface.
                    if (type.IsAssignableTo(typeof(ITraceResultSerializer)))
                    {
                        var serializer = (assembly.CreateInstance(type.FullName) as ITraceResultSerializer) !;
                        _serializers.Add(serializer);
                    }
                }
            }
            catch
            {
                // Ignored.
            }
        }
    }

    public void Write(TraceResult traceResult, string fileNamePrefix)
    {
        foreach (var serializer in _serializers)
        {
            using var to = new FileStream($"{fileNamePrefix}.{serializer.Format}", FileMode.Create);
            serializer.Serialize(traceResult, to);
        }
    }
}
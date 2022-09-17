using System.Reflection;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization;

public static class SerializerLoader
{
    public static List<ITraceResultSerializer> LoadSerializers(string directory)
    {
        List<ITraceResultSerializer> serializers = new();
        string[] plugins = Directory.GetFiles(directory, "*.dll");
        foreach (var plugin in plugins)
            try
            {
                var assembly = Assembly.LoadFrom(plugin);

                // Get all types, which implement interface.
                Type[] types = assembly.GetTypes();
                foreach (var type in types)
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

        return serializers;
    }
}
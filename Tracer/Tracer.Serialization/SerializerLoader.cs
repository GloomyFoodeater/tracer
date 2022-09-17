using System.Reflection;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization;

public static class SerializerLoader
{
    public static List<ITraceResultSerializer> LoadSerializers(string directory)
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
}
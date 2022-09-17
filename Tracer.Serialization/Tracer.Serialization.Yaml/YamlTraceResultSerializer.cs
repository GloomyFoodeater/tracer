using Tracer.Core;
using Tracer.Serialization.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Tracer.Serialization.Yaml;

public class YamlTraceResultSerializer : ITraceResultSerializer
{
    public string Format => "yaml";

    public void Serialize(TraceResult traceResult, Stream to)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .DisableAliases()
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build();
        using var writer = new StreamWriter(to);
        serializer.Serialize(writer, traceResult);
    }
}
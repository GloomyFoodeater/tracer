using Tracer.Core;
using Tracer.Serialization.Abstractions;
using XSerializer;

namespace Tracer.Serialization.Xml;

public class XmlTraceResultSerializer : ITraceResultSerializer
{
    public string Format => "xml";

    public void Serialize(TraceResult traceResult, Stream to)
    {
        XmlTraceResult wrapper = new(traceResult);
        XmlSerializer<XmlTraceResult> serializer = new(new XmlSerializationOptions().Indent());
        serializer.Serialize(to, wrapper);
    }
}
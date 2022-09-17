using Tracer.Core;
using Tracer.Serialization.Abstractions;
using XSerializer;

namespace Tracer.Serialization.Xml;

public class XmlTraceResultSerializer: ITraceResultSerializer
{
    public string Format => "xml";
    public void Serialize(TraceResult traceResult, Stream to)
    {
        XmlSerializer<TraceResult> serializer = new(new XmlSerializationOptions().Indent());
        serializer.Serialize(to, traceResult);
    }
}
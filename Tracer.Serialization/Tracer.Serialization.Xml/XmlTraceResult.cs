using System.Xml.Serialization;
using Tracer.Core;

namespace Tracer.Serialization.Xml;

[XmlRoot("root")]
public class XmlTraceResult
{
    public XmlTraceResult(TraceResult res)
    {
        foreach (ThreadInfo info in res.Threads)
        {
            Threads.Add(new XmlThread(info));
        }
    }

    [XmlElement("thread")] public List<XmlThread> Threads { get; set; } = new();
}
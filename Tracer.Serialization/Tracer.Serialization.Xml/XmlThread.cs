using System.Xml.Serialization;
using Tracer.Core;

namespace Tracer.Serialization.Xml;

public class XmlThread
{
    public XmlThread(ThreadInfo info)
    {
        Id = info.Id;
        Time = info.Time;
        foreach (MethodInfo method in info.Methods)
        {
            Methods.Add(new XmlMethod(method));
        }
    }

    [XmlAttribute("id")] public int Id { get; set; }

    [XmlAttribute("time")] public string Time { get; set; }

    [XmlElement("method")] public List<XmlMethod> Methods { get; set; } = new();
}
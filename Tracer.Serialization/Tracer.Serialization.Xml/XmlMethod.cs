using System.Xml.Serialization;
using Tracer.Core;

namespace Tracer.Serialization.Xml;

public class XmlMethod
{
    public XmlMethod(MethodInfo method)
    {
        Name = method.Name;
        Class = method.Class;
        Time = method.Time;
        if (method.Methods != null)
        {
            Methods = new();
            foreach (MethodInfo child in method.Methods)
            {
                Methods.Add(new XmlMethod(child));
            }
        }
    }

    [XmlAttribute("name")] public string Name { get; set; }

    [XmlAttribute("time")] public string Time { get; set; }

    [XmlAttribute("class")] public string Class { get; set; }

    [XmlElement("method")] public List<XmlMethod>? Methods { get; set; }
}
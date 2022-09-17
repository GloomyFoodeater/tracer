using System.Diagnostics;

namespace Tracer.Core;

public class MethodNode
{
    public MethodNode(string name, string @class, MethodNode? father)
    {
        Name = name;
        Class = @class;
        Father = father;
        Watch.Start();
    }

    public string Name { get; }
    
    public string Class { get; }
    
    public Stopwatch Watch { get; } = new();
    
    public MethodNode? Father { get; }
    
    public List<MethodNode> Children { get; } = new();
}
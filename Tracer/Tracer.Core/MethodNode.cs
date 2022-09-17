using System.Diagnostics;

namespace Tracer.Core;

internal class MethodNode
{
    public MethodNode(string name, string @class, MethodNode? father)
    {
        Name = name;
        Class = @class;
        Father = father;
        Watch.Start();
    }

    private string Name { get; }

    private string Class { get; }

    public Stopwatch Watch { get; } = new();

    public MethodNode? Father { get; }

    public List<MethodNode> Children { get; } = new();

    public MethodInfo ToMethodInfo()
    {
        // Convert children methods from node to thread recursively.
        List<MethodInfo> methods = new();
        foreach (MethodNode child in Children)
        {
            methods.Add(child.ToMethodInfo());
        }

        return new(Name, Class, Watch.Elapsed.TotalMilliseconds + "ms", methods);
    }
}
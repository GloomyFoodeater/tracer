namespace Tracer.Core;

public class ThreadNode
{
    public ThreadNode(int id)
    {
        Id = id;
    }

    public int Id { get; }
    
    public List<MethodNode> Methods { get; } = new();
    
    public MethodNode? Current { get; private set; }

    public void StartAdd(string name, string @class)
    {
        // Make method node.
        MethodNode methodNode = new(name, @class, Current);

        // Add method node to tree.
        if (Current == null)
        {
            Methods.Add(methodNode);
        }
        else
        {
            Current.Children.Add(methodNode);
        }

        // Move link from father to child.
        Current = methodNode;
    }

    public void StopAdd()
    {
        // Current can be null if StopAdd was called before StartAdd.
        if (Current != null)
        {
            // Finish updating info.
            Current.Watch.Stop();

            // Return link from child to father.
            Current = Current.Father;
        }
    }
}
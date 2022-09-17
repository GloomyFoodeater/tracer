using System.Collections.Concurrent;
using System.Diagnostics;
using Tracer.Core.TracerNodes;

namespace Tracer.Core;

public class Tracer : ITracer
{
    private readonly ConcurrentDictionary<int, ThreadNode> _threads = new();

    public void StartTrace()
    {
        // Get thread node to update.
        int tid = Thread.CurrentThread.ManagedThreadId;
        ThreadNode threadNode = _threads.GetOrAdd(tid, key => new ThreadNode(key));

        // Get caller info.
        StackTrace trace = new();
        StackFrame frame = trace.GetFrame(1)!;
        string name = frame.GetMethod()!.Name;
        string @class = frame.GetMethod()!.ReflectedType!.Name;

        // Start adding method to method tree.
        threadNode.StartAdd(name, @class);
    }

    public void StopTrace()
    {
        // Get thread node to update.
        int tid = Thread.CurrentThread.ManagedThreadId;
        if (_threads.TryGetValue(tid, out var threadNode))
        {
            // Stop adding method to method tree.
            threadNode.StopAdd();
        }
    }

    public TraceResult GetTraceResult()
    {
        // Convert thread nodes to infos.
        List<ThreadInfo> threadInfos = new();
        foreach (var (_, threadNode) in _threads)
        {
            threadInfos.Add(threadNode.ToThreadInfo());
        }

        return new(threadInfos);
    }
}
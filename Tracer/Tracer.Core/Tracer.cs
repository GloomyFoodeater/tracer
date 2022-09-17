using System.Diagnostics;

namespace Tracer.Core;

public class Tracer : ITracer
{
    private readonly List<ThreadNode> _threads = new();

    public void StartTrace()
    {
        // Get thread node to update.
        int tid = Thread.CurrentThread.ManagedThreadId;
        ThreadNode? threadNode = _threads.Find(node => node.Id == tid);

        // Add new thread node to list.
        if (threadNode == null)
        {
            lock (new object())
            {
                threadNode = new ThreadNode(tid);
                _threads.Add(threadNode);
            }
        }

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
        ThreadNode? threadNode = _threads.Find(node => node.Id == tid);

        // Stop adding method to method tree.
        threadNode?.StopAdd();
    }

    public TraceResult GetTraceResult()
    {
        // Covert thread nodes to infos.
        List<ThreadInfo> threadInfos = new();
        foreach (ThreadNode threadNode in _threads)
        {
            threadInfos.Add(threadNode.ToThreadInfo());
        }

        return new(threadInfos);
    }
}
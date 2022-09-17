using System.Diagnostics;
using Tracer.Core.TracerNodes;

namespace Tracer.Core;

public class Tracer : ITracer
{
    private readonly List<ThreadNode> _threads = new();

    private readonly object _threadsLock = new object();

    public void StartTrace()
    {
        // Get thread node to update.
        int tid = Thread.CurrentThread.ManagedThreadId;
        ThreadNode? threadNode;
        lock (_threadsLock)
        {
            threadNode = _threads.Find(node => node.Id == tid);

            // Add new thread node to list.
            if (threadNode == null)
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
        ThreadNode? threadNode;
        lock (_threadsLock)
        {
            threadNode = _threads.Find(node => node.Id == tid);
        }

        // Stop adding method to method tree.
        threadNode?.StopAdd();
    }

    public TraceResult GetTraceResult()
    {
        // Covert thread nodes to infos.
        List<ThreadInfo> threadInfos = new();
        lock (_threadsLock)
        {
            foreach (ThreadNode threadNode in _threads)
            {
                threadInfos.Add(threadNode.ToThreadInfo());
            }
        }

        return new(threadInfos);
    }
}
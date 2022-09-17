namespace Tracer.Core;

public record TraceResult(
    IReadOnlyList<ThreadInfo> Threads);
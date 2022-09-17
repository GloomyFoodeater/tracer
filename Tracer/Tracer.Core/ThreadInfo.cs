namespace Tracer.Core;

public record ThreadInfo(
    int Id,
    string Time,
    IReadOnlyList<MethodInfo> Methods);
namespace Tracer.Core;

public record MethodInfo(
    string Name,
    string Class,
    string Time,
    IReadOnlyList<MethodInfo>? Methods);
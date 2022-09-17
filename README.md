# Execution flow tracer
Measure time of methods' execution using system class StackTrace.

## Tracer
Tracer collects method name, caller class name, time of each method execution and
total execution time of analyzed methods in the same thread and returns result as
immutable tree of threads and methods.

## Usage
```C#
class Bar
{
  public void Foo(ITracer tracer)
  {
      tracer.StartTrace();

      // Your code

      tracer.StopTrace();    
  }
}
```
```C#
using Tracer.Core;

ITracer tracer = new Tracer.Core.Tracer();
Bar bar = new Bar();
bar.Foo(tracer);
TraceResult result = tracer.GetTraceResult();
```

## Serialization
Tracer.Serialization solution contains XML, JSON and YAML serializers of
trace result tree, which can be compiled and used as plugins.
### JSON
```JSON
{
  "threads": [
    {
      "id": 1,
      "time": "0ms",
      "methods": [
        {
          "name": "Foo",
          "class": "Bar",
          "time": "0ms"
        }
      ]
    }
  ]
}
```
### XML
```XML
<?xml version="1.0" encoding="utf-8"?>
<root xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <thread id="1" time="0ms">
    <method name="Foo" time="0ms" class="Bar" />
  </thread>
</root>
```
### YAML
```YAML
threads:
- id: 1
  time: 0ms
  methods:
  - name: Foo
    class: Bar
    time: 0ms
```

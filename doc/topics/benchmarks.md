---
uid: urn:topics:benchmarks
---

## Addax - Benchmarks

<p />

The following benchmarks reflect the approximate time and memory required to process `1,048,576` fields:

<p />

```txt 
.NET 8.0.0 (8.0.23.53103)

| Method                         | Mean        | Median      | Min         | Max         | Op/s   | Allocated |
|------------------------------- |------------:|------------:|------------:|------------:|-------:|----------:|
| 'read field: empty'            |  7,232.5 us |  7,256.1 us |  7,150.9 us |  7,336.2 us | 138.27 |     389 B |
| 'read field: regular'          | 18,323.2 us | 18,279.8 us | 18,152.8 us | 18,554.7 us |  54.58 |     406 B |
| 'read field: escaped'          | 50,433.9 us | 50,409.4 us | 50,219.8 us | 50,809.0 us |  19.83 |     447 B |
| 'read field: empty (async)'    |  7,614.2 us |  7,637.3 us |  7,487.2 us |  7,747.4 us | 131.33 |     389 B |
| 'read field: regular (async)'  | 19,077.0 us | 19,116.3 us | 18,918.2 us | 19,231.4 us |  52.42 |     406 B |
| 'read field: escaped (async)'  | 51,548.7 us | 51,465.8 us | 51,305.2 us | 51,979.0 us |  19.40 |     395 B |

| Method                         | Mean        | Median      | Min         | Max         | Op/s   | Allocated |
|------------------------------- |------------:|------------:|------------:|------------:|-------:|----------:|
| 'write field: empty'           |  5,401.2 us |  5,387.5 us |  5,325.0 us |  5,523.5 us | 185.15 |     389 B |
| 'write field: regular'         | 10,059.8 us | 10,060.6 us | 10,031.1 us | 10,101.1 us |  99.41 |     385 B |
| 'write field: escaped'         | 14,677.5 us | 14,675.2 us | 14,640.0 us | 14,733.7 us |  68.13 |     395 B |
| 'write field: empty (async)'   | 10,243.5 us | 10,240.5 us | 10,199.4 us | 10,303.8 us |  97.62 |     395 B |
| 'write field: regular (async)' | 15,227.2 us | 15,232.5 us | 15,163.4 us | 15,290.2 us |  65.67 |     395 B |
| 'write field: escaped (async)' | 19,948.4 us | 19,946.0 us | 19,895.2 us | 19,998.9 us |  50.13 |     406 B |

Mean      : Arithmetic mean of all measurements
Error     : Half of 99.9% confidence interval
StdDev    : Standard deviation of all measurements
Median    : Value separating the higher half of all measurements (50th percentile)
Min       : Minimum
Max       : Maximum
Op/s      : Operation per second
Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
1 μs      : 1 Microsecond (0.000001 sec)
```

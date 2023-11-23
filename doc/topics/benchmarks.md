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
| 'read field: empty'            |  6,738.6 μs |  6,756.1 μs |  6,609.2 μs |  6,906.1 μs | 148.40 |     389 B |
| 'read field: regular'          | 17,973.0 μs | 17,939.9 μs | 17,783.3 μs | 18,286.9 μs |  55.64 |     406 B |
| 'read field: escaped'          | 33,030.4 μs | 32,845.8 μs | 32,497.5 μs | 34,234.7 μs |  30.28 |     430 B |
| 'read field: empty (async)'    |  7,253.0 μs |  7,235.0 μs |  7,203.0 μs |  7,328.2 μs | 137.87 |     389 B |
| 'read field: regular (async)'  | 18,483.3 μs | 18,481.3 μs | 18,430.0 μs | 18,540.9 μs |  54.10 |     406 B |
| 'read field: escaped (async)'  | 35,669.9 μs | 35,640.6 μs | 35,502.1 μs | 35,841.1 μs |  28.03 |     430 B |

| Method                         | Mean        | Median      | Min         | Max         | Op/s   | Allocated |
|------------------------------- |------------:|------------:|------------:|------------:|-------:|----------:|
| 'write field: empty'           |  5,341.3 μs |  5,352.1 μs |  5,256.9 μs |  5,423.7 μs | 187.22 |     277 B |
| 'write field: regular'         |  9,236.5 μs |  9,238.9 μs |  9,157.1 μs |  9,351.6 μs | 108.27 |     395 B |
| 'write field: escaped'         | 14,607.3 μs | 14,603.4 μs | 14,588.9 μs | 14,633.8 μs |  68.46 |     395 B |
| 'write field: empty (async)'   | 10,284.7 μs | 10,270.8 μs | 10,175.8 μs | 10,418.4 μs |  97.23 |     283 B |
| 'write field: regular (async)' | 14,646.5 μs | 14,634.4 μs | 14,588.9 μs | 14,711.1 μs |  68.28 |     385 B |
| 'write field: escaped (async)' | 20,475.2 μs | 20,501.8 μs | 20,289.4 μs | 20,557.1 μs |  48.84 |     406 B |

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

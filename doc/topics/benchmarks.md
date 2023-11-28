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
| 'read field: empty'            |  7,232.5 μs |  7,256.1 μs |  7,150.9 μs |  7,336.2 μs | 138.27 |     389 B |
| 'read field: regular'          | 18,323.2 μs | 18,279.8 μs | 18,152.8 μs | 18,554.7 μs |  54.58 |     406 B |
| 'read field: escaped'          | 50,433.9 μs | 50,409.4 μs | 50,219.8 μs | 50,809.0 μs |  19.83 |     447 B |
| 'read field: empty (async)'    |  7,614.2 μs |  7,637.3 μs |  7,487.2 μs |  7,747.4 μs | 131.33 |     389 B |
| 'read field: regular (async)'  | 19,077.0 μs | 19,116.3 μs | 18,918.2 μs | 19,231.4 μs |  52.42 |     406 B |
| 'read field: escaped (async)'  | 51,548.7 μs | 51,465.8 μs | 51,305.2 μs | 51,979.0 μs |  19.40 |     395 B |

| Method                         | Mean        | Median      | Min         | Max         | Op/s   | Allocated |
|------------------------------- |------------:|------------:|------------:|------------:|-------:|----------:|
| 'write field: empty'           |  5,401.2 μs |  5,387.5 μs |  5,325.0 μs |  5,523.5 μs | 185.15 |     389 B |
| 'write field: regular'         | 10,059.8 μs | 10,060.6 μs | 10,031.1 μs | 10,101.1 μs |  99.41 |     385 B |
| 'write field: escaped'         | 14,677.5 μs | 14,675.2 μs | 14,640.0 μs | 14,733.7 μs |  68.13 |     395 B |
| 'write field: empty (async)'   | 10,243.5 μs | 10,240.5 μs | 10,199.4 μs | 10,303.8 μs |  97.62 |     395 B |
| 'write field: regular (async)' | 15,227.2 μs | 15,232.5 μs | 15,163.4 μs | 15,290.2 μs |  65.67 |     395 B |
| 'write field: escaped (async)' | 19,948.4 μs | 19,946.0 μs | 19,895.2 μs | 19,998.9 μs |  50.13 |     406 B |

Mean      : Arithmetic mean of all measurements
Median    : Value separating the higher half of all measurements (50th percentile)
Min       : Minimum
Max       : Maximum
Op/s      : Operation per second
Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
1 μs      : 1 Microsecond (0.000001 sec)
```

<p />

### References

<p />

- [Addax - Performance benchmarks for .NET libraries that work with CSV files](https://github.com/alexanderkozlenko/addax-benchmarks)

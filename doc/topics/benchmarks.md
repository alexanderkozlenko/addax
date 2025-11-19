---
uid: urn:topics:benchmarks
---

# Addax - Benchmarks

<p />

The following benchmarks reflect the approximate time and memory required to process `1,048,576` fields:

<p />

```txt 
.NET 10.0.0 (10.0.0, 10.0.25.52411)

| Method                          | Mean      | Median    | Min       | Max       | Op/s   | Allocated |
|-------------------------------- |----------:|----------:|----------:|----------:|-------:|----------:|
| 'reading field: sync, empty'    |  5.096 ms |  5.073 ms |  5.038 ms |  5.241 ms | 196.24 |     384 B |
| 'reading field: sync, regular'  | 13.220 ms | 13.222 ms | 13.211 ms | 13.230 ms |  75.64 |     425 B |
| 'reading field: sync, escaped'  | 38.709 ms | 38.674 ms | 38.623 ms | 38.866 ms |  25.83 |     591 B |
| 'reading field: async, empty'   |  5.851 ms |  5.818 ms |  5.806 ms |  6.008 ms | 170.92 |     405 B |
| 'reading field: async, regular' | 14.405 ms | 14.388 ms | 14.371 ms | 14.521 ms |  69.42 |     426 B |
| 'reading field: async, escaped' | 39.258 ms | 39.263 ms | 38.920 ms | 39.629 ms |  25.47 |     613 B |
| 'writing field: sync, empty'    |  3.187 ms |  3.193 ms |  3.140 ms |  3.199 ms | 313.80 |     384 B |
| 'writing field: sync, regular'  |  7.318 ms |  7.319 ms |  7.306 ms |  7.324 ms | 136.64 |     407 B |
| 'writing field: sync, escaped'  | 12.327 ms | 12.329 ms | 12.270 ms | 12.365 ms |  81.12 |     425 B |
| 'writing field: async, empty'   |  8.274 ms |  8.274 ms |  8.214 ms |  8.315 ms | 120.87 |     426 B |
| 'writing field: async, regular' | 12.584 ms | 12.579 ms | 12.567 ms | 12.608 ms |  79.47 |     430 B |
| 'writing field: async, escaped' | 17.664 ms | 17.611 ms | 17.479 ms | 17.938 ms |  56.61 |     468 B |

Mean      : Arithmetic mean of all measurements
Median    : Value separating the higher half of all measurements (50th percentile)
Min       : Minimum
Max       : Maximum
Op/s      : Operation per second
Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
1 ms      : 1 Millisecond (0.001 sec)
```

<p />

## References

<p />

- [Addax - Performance benchmarks for .NET libraries that work with CSV files](https://github.com/alexanderkozlenko/addax-benchmarks)

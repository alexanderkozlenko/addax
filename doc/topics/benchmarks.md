---
uid: urn:topics:benchmarks
---

# Addax - Benchmarks

<p />

The following benchmarks reflect the approximate time and memory required to process 1,048,576 fields:

<p />

```txt 
.NET 8.0.0 (8.0.23.53103)

| Method                         | Mean      | Median    | Min       | Max       | Op/s   | Allocated |
|------------------------------- |----------:|----------:|----------:|----------:|-------:|----------:|
| 'read field: empty'            |  6.620 ms |  6.618 ms |  6.531 ms |  6.743 ms | 151.06 |     389 B |
| 'read field: regular'          | 14.482 ms | 14.378 ms | 14.325 ms | 14.805 ms |  69.05 |     395 B |
| 'read field: escaped'          | 40.464 ms | 40.431 ms | 40.155 ms | 40.854 ms |  24.71 |     389 B |
| 'read field: empty (async)'    |  7.059 ms |  7.049 ms |  6.994 ms |  7.157 ms | 141.66 |     389 B |
| 'read field: regular (async)'  | 15.484 ms | 15.482 ms | 15.427 ms | 15.564 ms |  64.58 |     395 B |
| 'read field: escaped (async)'  | 40.815 ms | 40.822 ms | 40.695 ms | 40.909 ms |  24.50 |     437 B |

| Method                         | Mean      | Median    | Min       | Max       | Op/s   | Allocated |
|------------------------------- |----------:|----------:|----------:|----------:|-------:|----------:|
| 'write field: empty'           |  5.367 ms |  5.366 ms |  5.343 ms |  5.398 ms | 186.34 |     389 B |
| 'write field: regular'         |  9.966 ms |  9.960 ms |  9.928 ms | 10.005 ms | 100.34 |     395 B |
| 'write field: escaped'         | 15.120 ms | 15.124 ms | 15.006 ms | 15.285 ms |  66.14 |     395 B |
| 'write field: empty (async)'   | 10.133 ms | 10.126 ms | 10.045 ms | 10.212 ms |  98.69 |     395 B |
| 'write field: regular (async)' | 15.075 ms | 15.087 ms | 15.004 ms | 15.152 ms |  66.33 |     395 B |
| 'write field: escaped (async)' | 20.655 ms | 20.655 ms | 20.567 ms | 20.754 ms |  48.41 |     406 B |

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

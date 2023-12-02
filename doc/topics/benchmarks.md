---
uid: urn:topics:benchmarks
---

## Addax - Benchmarks

<p />

The following benchmarks reflect the approximate time and memory required to process 1,048,576 fields:

<p />

```txt 
.NET 8.0.0 (8.0.23.53103)

| Method                         | Mean      | Median    | Min       | Max       | Op/s   | Allocated |
|------------------------------- |----------:|----------:|----------:|----------:|-------:|----------:|
| 'read field: empty'            |  7.320 ms |  7.358 ms |  7.107 ms |  7.404 ms | 136.62 |     389 B |
| 'read field: regular'          | 18.033 ms | 18.030 ms | 17.958 ms | 18.134 ms |  55.46 |     386 B |
| 'read field: escaped'          | 49.936 ms | 49.929 ms | 49.797 ms | 50.194 ms |  20.03 |     453 B |
| 'read field: empty (async)'    |  7.505 ms |  7.502 ms |  7.471 ms |  7.550 ms | 133.24 |     389 B |
| 'read field: regular (async)'  | 19.129 ms | 19.116 ms | 18.922 ms | 19.385 ms |  52.28 |     406 B |
| 'read field: escaped (async)'  | 50.724 ms | 50.681 ms | 50.500 ms | 51.067 ms |  19.71 |     390 B |

| Method                         | Mean      | Median    | Min       | Max       | Op/s   | Allocated |
|------------------------------- |----------:|----------:|----------:|----------:|-------:|----------:|
| 'write field: empty'           |  5.329 ms |  5.325 ms |  5.287 ms |  5.363 ms | 187.66 |     384 B |
| 'write field: regular'         |  9.968 ms |  9.969 ms |  9.952 ms |  9.978 ms | 100.32 |     395 B |
| 'write field: escaped'         | 14.955 ms | 14.954 ms | 14.914 ms | 15.006 ms |  66.87 |     395 B |
| 'write field: empty (async)'   | 10.193 ms | 10.202 ms | 10.157 ms | 10.224 ms |  98.11 |     395 B |
| 'write field: regular (async)' | 15.032 ms | 15.033 ms | 14.997 ms | 15.068 ms |  66.52 |     395 B |
| 'write field: escaped (async)' | 20.378 ms | 20.381 ms | 20.319 ms | 20.464 ms |  49.07 |     406 B |

Mean      : Arithmetic mean of all measurements
Median    : Value separating the higher half of all measurements (50th percentile)
Min       : Minimum
Max       : Maximum
Op/s      : Operation per second
Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
1 ms      : 1 Millisecond (0.001 sec)
```

<p />

### References

<p />

- [Addax - Performance benchmarks for .NET libraries that work with CSV files](https://github.com/alexanderkozlenko/addax-benchmarks)

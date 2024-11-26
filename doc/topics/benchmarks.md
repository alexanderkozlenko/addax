---
uid: urn:topics:benchmarks
---

# Addax - Benchmarks

<p />

The following benchmarks reflect the approximate time and memory required to process 1,048,576 fields:

<p />

```txt 
.NET 9.0.0 (9.0.24.52809)

| Method                         | Mean      | Median    | Op/s   | Allocated |
|------------------------------- |----------:|----------:|-------:|----------:|
| 'read field: empty'            |  6.628 ms |  6.618 ms | 150.88 |     387 B |
| 'read field: regular'          | 15.305 ms | 15.286 ms |  65.34 |     479 B |
| 'read field: escaped'          | 45.170 ms | 45.105 ms |  22.14 |     661 B |
| 'read field: empty (async)'    |  7.087 ms |  7.107 ms | 141.11 |     408 B |
| 'read field: regular (async)'  | 15.772 ms | 15.728 ms |  63.40 |     479 B |
| 'read field: escaped (async)'  | 45.744 ms | 45.856 ms |  21.86 |     637 B |

| Method                         | Mean      | Median    | Op/s   | Allocated |
|------------------------------- |----------:|----------:|-------:|----------:|
| 'write field: empty'           |  4.959 ms |  4.949 ms | 201.67 |     400 B |
| 'write field: regular'         |  9.324 ms |  9.325 ms | 107.25 |     432 B |
| 'write field: escaped'         | 13.783 ms | 13.775 ms |  72.55 |     432 B |
| 'write field: empty (async)'   |  9.529 ms |  9.530 ms | 104.95 |     432 B |
| 'write field: regular (async)' | 14.466 ms | 14.483 ms |  69.13 |     427 B |
| 'write field: escaped (async)' | 19.326 ms | 19.342 ms |  51.74 |     386 B |

Mean      : Arithmetic mean of all measurements
Median    : Value separating the higher half of all measurements (50th percentile)
Op/s      : Operation per second
Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
1 ms      : 1 Millisecond (0.001 sec)
```

<p />

## References

<p />

- [Addax - Performance benchmarks for .NET libraries that work with CSV files](https://github.com/alexanderkozlenko/addax-benchmarks)

## Addax - Benchmarks

<p />

```txt
.NET SDK 8.0.100-rc.2.23502.2, BenchmarkDotNet v0.13.10
```

<p />

| Method                   | Mean        | Error     | StdDev    | Median      | Min         | Max         | Op/s   | Allocated |
|------------------------- |------------:|----------:|----------:|------------:|------------:|------------:|-------:|----------:|
| 'read - empty'           |  6,915.3 µs | 135.99 µs | 156.61 µs |  6,880.8 µs |  6,659.2 µs |  7,175.2 µs | 144.61 |     389 B |
| 'read - normal'          | 18,521.3 µs | 248.62 µs | 232.56 µs | 18,495.0 µs | 18,235.7 µs | 18,994.2 µs |  53.99 |     406 B |
| 'read - escaped'         | 33,057.6 µs | 222.56 µs | 173.76 µs | 32,980.2 µs | 32,878.6 µs | 33,417.8 µs |  30.25 |     427 B |
| 'read - empty (saync)'   |  7,354.0 µs |  68.49 µs |  60.72 µs |  7,327.3 µs |  7,292.0 µs |  7,480.3 µs | 135.98 |     389 B |
| 'read - normal (async)'  | 18,709.2 µs |  25.89 µs |  22.95 µs | 18,713.0 µs | 18,646.0 µs | 18,735.8 µs |  53.45 |     406 B |
| 'read - escaped (async)' | 36,647.7 µs | 426.65 µs | 399.09 µs | 36,734.1 µs | 36,039.5 µs | 37,172.7 µs |  27.29 |     433 B |

<p />

| Method                    | Mean        | Error     | StdDev   | Median      | Min         | Max         | Op/s   | Allocated |
|-------------------------- |------------:|----------:|---------:|------------:|------------:|------------:|-------:|----------:|
| 'write - empty'           |  5,384.5 µs |  25.95 µs | 24.28 µs |  5,379.4 µs |  5,353.6 µs |  5,429.3 µs | 185.72 |     277 B |
| 'write - normal'          |  9,739.2 µs |  29.63 µs | 26.27 µs |  9,744.2 µs |  9,690.4 µs |  9,773.9 µs | 102.68 |     395 B |
| 'write - escaped'         | 15,129.0 µs |  31.95 µs | 28.32 µs | 15,122.8 µs | 15,088.4 µs | 15,191.1 µs |  66.10 |     395 B |
| 'write - empty (async)'   | 10,151.7 µs |  56.37 µs | 49.97 µs | 10,147.4 µs | 10,087.4 µs | 10,272.9 µs |  98.51 |     283 B |
| 'write - normal (async)'  | 14,848.0 µs |  60.42 µs | 56.52 µs | 14,846.0 µs | 14,770.8 µs | 14,971.1 µs |  67.35 |     395 B |
| 'write - escaped (async)' | 20,566.5 µs | 118.96 µs | 99.33 µs | 20,535.4 µs | 20,448.6 µs | 20,799.8 µs |  48.62 |     406 B |

<p />

```txt
Mean      : Arithmetic mean of all measurements
Error     : Half of 99.9% confidence interval
StdDev    : Standard deviation of all measurements
Median    : Value separating the higher half of all measurements (50th percentile)
Min       : Minimum
Max       : Maximum
Op/s      : Operation per second
Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
1 µs      : 1 Microsecond (0.000001 sec)
```

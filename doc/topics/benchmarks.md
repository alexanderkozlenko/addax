## Addax - Benchmarks

<p />

```txt 
.NET 8.0.0 (8.0.23.53103)

| Method                           | Mean        | Median      | Min         | Max         | Op/s   | Allocated |
|--------------------------------- |------------:|------------:|------------:|------------:|-------:|----------:|
| 'read string - empty'            |  7,176.5 us |  7,186.8 us |  7,082.7 us |  7,246.8 us | 139.34 |     389 B |
| 'read string - regular'          | 18,054.2 us | 18,053.4 us | 18,019.4 us | 18,084.6 us |  55.39 |     406 B |
| 'read string - escaped'          | 33,885.7 us | 33,849.9 us | 33,788.8 us | 34,088.2 us |  29.51 |     430 B |
| 'read string - empty (saync)'    |  7,374.2 us |  7,372.1 us |  7,366.2 us |  7,387.0 us | 135.61 |     389 B |
| 'read string - regular (async)'  | 18,727.4 us | 18,726.7 us | 18,690.5 us | 18,769.1 us |  53.40 |     406 B |
| 'read string - escaped (async)'  | 36,717.3 us | 36,741.7 us | 36,439.5 us | 36,937.6 us |  27.24 |     433 B |

| Method                           | Mean        | Median      | Min         | Max         | Op/s   | Allocated |
|--------------------------------- |------------:|------------:|------------:|------------:|-------:|----------:|
| 'write string - empty'           |  5,417.2 us |  5,404.4 us |  5,340.0 us |  5,503.8 us | 184.60 |     277 B |
| 'write string - regular'         |  9,931.1 us |  9,935.4 us |  9,832.0 us |  9,954.2 us | 100.69 |     395 B |
| 'write string - escaped'         | 14,797.1 us | 14,799.1 us | 14,746.7 us | 14,829.9 us |  67.58 |     395 B |
| 'write string - empty (async)'   | 10,216.9 us | 10,219.3 us | 10,192.3 us | 10,240.4 us |  97.88 |     283 B |
| 'write string - regular (async)' | 14,833.5 us | 14,832.2 us | 14,818.1 us | 14,853.4 us |  67.41 |     395 B |
| 'write string - escaped (async)' | 20,071.4 us | 20,086.1 us | 19,998.3 us | 20,115.6 us |  49.82 |     406 B |

Mean      : Arithmetic mean of all measurements
Error     : Half of 99.9% confidence interval
StdDev    : Standard deviation of all measurements
Median    : Value separating the higher half of all measurements (50th percentile)
Min       : Minimum
Max       : Maximum
Op/s      : Operation per second
Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
1 us      : 1 Microsecond (0.000001 sec)
```

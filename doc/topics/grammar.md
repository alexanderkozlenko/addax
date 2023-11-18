## Addax - Grammar

<p />

The engine is based on the following generalized grammar for tabular data:

<p />

```ebnf
stream = record, {tt, record};
record = field, {dt, field} | at, {char} - tt;
field = {char} - (tt | dt | qt) | qt, {{char} - (qt | et) | et, qt | et, et}, qt;

at = achar;
et = echar;
qt = qchar;
dt = dchar;
tt = tchar1 | tchar1, tchar2;

achar = char - (tchar1 | tchar2 | dchar | qchar | echar);
echar = char - (tchar1 | tchar2 | dchar);
qchar = char - (tchar1 | tchar2 | dchar);
dchar = char - (tchar1 | tchar2);
tchar2 = char - tchar1;
tchar1 = char;

char = ? %x00-10FFFF ?;
```

<p />

> [!NOTE]
> Fields with line terminator characters are written as escaped to ensure compatibility with more specific dialects, such as RFC 4180.

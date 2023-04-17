// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Primitives;

internal static class Singleton<T>
    where T : class, new()
{
    public static readonly T Instance = new();
}

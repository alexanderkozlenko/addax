using System;

namespace Addax.Formats.Tabular;

public sealed class TabularRecordAttribute : Attribute
{
}

public sealed class TabularFieldNameAttribute : Attribute
{
    public TabularFieldNameAttribute(string name)
    {
    }
}

public sealed class TabularFieldOrderAttribute : Attribute
{
    public TabularFieldOrderAttribute(int order)
    {
    }
}

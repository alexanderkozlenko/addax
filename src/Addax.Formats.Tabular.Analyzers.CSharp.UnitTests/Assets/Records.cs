using Addax.Formats.Tabular;

[TabularRecord]
public class Handler0
{
    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
        set;
    }
}

namespace MyNamespace
{
    [TabularRecord]
    public class ClassRecord
    {
        [TabularFieldOrder(0)]
        public int MyValue0
        {
            get;
            set;
        }
    }

    [TabularRecord]
    public struct StructRecord
    {
        [TabularFieldOrder(0)]
        public string? MyValue0;
    }

    [TabularRecord]
    public sealed record InitOnlyRecord
    {
        [TabularFieldOrder(0)]
        public double MyValue0
        {
            get;
            init;
        }
    }

    [TabularRecord]
    internal class InternalRecord
    {
        [TabularFieldOrder(0)]
        public int MyValue0
        {
            get;
            set;
        }
    }

    public class OuterType
    {
        [TabularRecord]
        internal class InternalNestedRecord
        {
            [TabularFieldOrder(0)]
            public int MyValue0
            {
                get;
                set;
            }
        }

        [TabularRecord]
        protected internal class ProtectedInternalNestedRecord
        {
            [TabularFieldOrder(0)]
            public int MyValue0
            {
                get;
                set;
            }
        }

        [TabularRecord]
        private class PrivateNestedRecord
        {
            [TabularFieldOrder(0)]
            public int MyValue0
            {
                get;
                set;
            }
        }

        private class PrivateOuterType
        {
            [TabularRecord]
            public class NestedInPrivateRecord
            {
                [TabularFieldOrder(0)]
                public int MyValue0
                {
                    get;
                    set;
                }
            }
        }

        [TabularRecord]
        protected class ProtectedNestedRecord
        {
            [TabularFieldOrder(0)]
            public int MyValue0
            {
                get;
                set;
            }
        }
    }

    [TabularRecord]
    public static class StaticRecord
    {
    }

    [TabularRecord]
    public ref struct RefStructRecord
    {
        [TabularFieldOrder(0)]
        public int MyValue0;
    }

    [TabularRecord]
    public class GenericRecord<T>
    {
        [TabularFieldOrder(0)]
        public int MyValue0
        {
            get;
            set;
        }
    }

    public class GenericOuterType<T>
    {
        [TabularRecord]
        public class NestedInGenericRecord
        {
            [TabularFieldOrder(0)]
            public int MyValue0
            {
                get;
                set;
            }
        }
    }

    [TabularRecord]
    file class FileLocalRecord
    {
        [TabularFieldOrder(0)]
        public int MyValue0
        {
            get;
            set;
        }
    }

    file class FileLocalOuterType
    {
        [TabularRecord]
        public class NestedInFileLocalRecord
        {
            [TabularFieldOrder(0)]
            public int MyValue0
            {
                get;
                set;
            }
        }
    }
}

namespace MyOtherNamespace
{
    [TabularRecord]
    public class ClassRecord
    {
        [TabularFieldOrder(0)]
        public int MyValue0
        {
            get;
            set;
        }
    }
}

namespace Addax.Formats.Tabular.Handlers
{
    [TabularRecord]
    public class Handler
    {
        [TabularFieldOrder(0)]
        public int MyValue0
        {
            get;
            set;
        }
    }

    [TabularRecord]
    public class Registration
    {
        [TabularFieldOrder(0)]
        public int MyValue0
        {
            get;
            set;
        }
    }
}

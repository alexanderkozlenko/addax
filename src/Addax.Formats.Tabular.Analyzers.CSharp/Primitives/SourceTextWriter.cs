// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace Addax.Formats.Tabular.Analyzers.CSharp.Primitives
{
    internal sealed class SourceTextWriter
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private readonly Encoding _encoding;
        private readonly char _indentChar;
        private readonly int _indentSize;

        public SourceTextWriter(Encoding encoding, char indentChar, int indentSize)
        {
            Debug.Assert(encoding != null);
            Debug.Assert(indentSize >= 0);

            _encoding = encoding;
            _indentChar = indentChar;
            _indentSize = indentSize;
        }

        public void WriteLine(string value)
        {
            Debug.Assert(value != null);

            _stringBuilder.Append(_indentChar, Indent * _indentSize);
            _stringBuilder.AppendLine(value);
        }

        public void WriteLine()
        {
            _stringBuilder.AppendLine();
        }

        public SourceText ToSourceText()
        {
            return SourceText.From(_stringBuilder.ToString(), _encoding);
        }

        public void Reset()
        {
            Indent = 0;

            _stringBuilder.Clear();
        }

        public int Indent
        {
            get;
            set;
        }
    }
}

// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal sealed class SourceTextWriter : IDisposable
    {
        private readonly MemoryStream _memoryStream = new MemoryStream();
        private readonly StreamWriter _streamWriter;
        private readonly char _indentChar;
        private readonly int _indentSize;

        public SourceTextWriter(Encoding encoding, char indentChar, int indentSize)
        {
            Debug.Assert(encoding != null);
            Debug.Assert(indentSize >= 0);

            _streamWriter = new StreamWriter(_memoryStream, encoding);
            _indentChar = indentChar;
            _indentSize = indentSize;
        }

        public void Dispose()
        {
            _streamWriter.Dispose();
        }

        public void WriteLine(string value)
        {
            Debug.Assert(value != null);

            var indentation = Indent * _indentSize;

            for (var i = 0; i < indentation; i++)
            {
                _streamWriter.Write(_indentChar);
            }

            _streamWriter.WriteLine(value);
        }

        public void WriteLine()
        {
            _streamWriter.WriteLine();
        }

        public SourceText ToSourceText()
        {
            _streamWriter.Flush();

            return SourceText.From(_memoryStream, _streamWriter.Encoding, canBeEmbedded: true);
        }

        public void Reset()
        {
            Indent = 0;

            _memoryStream.SetLength(0);
        }

        public int Indent
        {
            get;
            set;
        }
    }
}

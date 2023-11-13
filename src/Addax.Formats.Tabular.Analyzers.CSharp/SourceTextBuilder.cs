// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal sealed class SourceTextBuilder : IDisposable
    {
        private readonly MemoryStream _memoryStream = new MemoryStream();
        private readonly StreamWriter _streamWriter;
        private readonly Encoding _encoding;
        private readonly int _indentSize;
        private readonly char _indentChar;

        private int _indentLevel;

        public SourceTextBuilder(char indentChar, int indentSize, Encoding encoding)
        {
            _indentChar = indentChar;
            _indentSize = indentSize;
            _encoding = encoding;
            _streamWriter = new StreamWriter(_memoryStream, _encoding);
        }

        public void Dispose()
        {
            _streamWriter.Dispose();
        }

        public void Append(string value = null)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var indentValue = _indentLevel * _indentSize;

                for (var i = 0; i < indentValue; i++)
                {
                    _streamWriter.Write(_indentChar);
                }

                _streamWriter.WriteLine(value);
            }
            else
            {
                _streamWriter.WriteLine();
            }
        }

        public SourceText ToSourceText()
        {
            _streamWriter.Flush();

            return SourceText.From(_memoryStream, _encoding, canBeEmbedded: true);
        }

        public int IndentLevel
        {
            get
            {
                return _indentLevel;
            }
            set
            {
                _indentLevel = value;
            }
        }
    }
}

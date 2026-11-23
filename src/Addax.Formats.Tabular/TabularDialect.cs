// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Addax.Formats.Tabular.Primitives;

namespace Addax.Formats.Tabular;

/// <summary>Specifies how to read and write tabular data. This class cannot be inherited.</summary>
/// <remarks>All public members of the type are thread-safe and may be used concurrently from multiple threads.</remarks>
public sealed class TabularDialect
{
    internal readonly TabularSearchValues SearchValues;

    private TabularDialect(string lineTerminator, char delimiter, char quoteChar, char escapeChar, char? commentPrefix)
    {
        ArgumentNullException.ThrowIfNull(lineTerminator);

        if ((lineTerminator.Length is not (1 or 2)) || ((lineTerminator.Length == 2) && (lineTerminator[0] == lineTerminator[1])))
        {
            ThrowTokenException(nameof(lineTerminator));
        }

        if (lineTerminator.Contains(delimiter, StringComparison.Ordinal))
        {
            ThrowTokenException(nameof(delimiter));
        }

        if (lineTerminator.Contains(quoteChar, StringComparison.Ordinal) || (quoteChar == delimiter))
        {
            ThrowTokenException(nameof(quoteChar));
        }

        if (lineTerminator.Contains(escapeChar, StringComparison.Ordinal) || (escapeChar == delimiter))
        {
            ThrowTokenException(nameof(escapeChar));
        }

        if (commentPrefix.HasValue)
        {
            var commentPrefixValue = commentPrefix.Value;

            if (lineTerminator.Contains(commentPrefixValue, StringComparison.Ordinal) ||
                (commentPrefixValue == delimiter) ||
                (commentPrefixValue == quoteChar) ||
                (commentPrefixValue == escapeChar))
            {
                ThrowTokenException(nameof(commentPrefix));
            }
        }

        LineTerminator = lineTerminator;
        Delimiter = delimiter;
        QuoteChar = quoteChar;
        EscapeChar = escapeChar;
        CommentPrefix = commentPrefix;

        SearchValues = new(lineTerminator, delimiter, quoteChar, escapeChar);

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowTokenException(string paramName)
        {
            throw new ArgumentException("The token value is not valid for this dialect.", paramName);
        }
    }

    /// <summary>Initializes a new instance of the <see cref="TabularDialect" /> class with the specified tokens.</summary>
    /// <param name="lineTerminator">The character sequence that is used to separate records. Must have one or two distinct characters.</param>
    /// <param name="delimiter">The character that is used to separate fields. Must differ from line terminator characters.</param>
    /// <param name="quoteChar">The character that is used to surround an escaped field. Must differ from line terminator and delimiter characters.</param>
    /// <param name="escapeChar">The character that is used to escape the quote character and itself within an escaped field. Must differ from line terminator and delimiter characters.</param>
    /// <param name="commentPrefix">The character that is used at the beginning of a record to indicate that the record is a comment. Must differ from line terminator, delimiter, quote, and escape characters.</param>
    /// <exception cref="ArgumentException">A token value is not valid for this dialect.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="lineTerminator" /> is <see langword="null" />.</exception>
    public TabularDialect(string lineTerminator, char delimiter, char quoteChar, char escapeChar, char commentPrefix)
        : this(lineTerminator, delimiter, quoteChar, escapeChar, (char?)commentPrefix)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularDialect" /> class with the specified tokens.</summary>
    /// <param name="lineTerminator">The character sequence that is used to separate records. Must have one or two distinct characters.</param>
    /// <param name="delimiter">The character that is used to separate fields. Must differ from line terminator characters.</param>
    /// <param name="quoteChar">The character that is used to surround an escaped field. Must differ from line terminator and delimiter characters.</param>
    /// <param name="escapeChar">The character that is used to escape the quote character and itself within an escaped field. Must differ from line terminator and delimiter characters.</param>
    /// <exception cref="ArgumentException">A token value is not valid for this dialect.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="lineTerminator" /> is <see langword="null" />.</exception>
    public TabularDialect(string lineTerminator, char delimiter, char quoteChar, char escapeChar)
        : this(lineTerminator, delimiter, quoteChar, escapeChar, null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularDialect" /> class with the specified tokens.</summary>
    /// <param name="lineTerminator">The character sequence that is used to separate records. Must have one or two distinct characters.</param>
    /// <param name="delimiter">The character that is used to separate fields. Must differ from line terminator characters.</param>
    /// <param name="quoteChar">The character that is used to surround an escaped field and escape itself within an escaped field. Must differ from line terminator and delimiter characters.</param>
    /// <exception cref="ArgumentException">A token value is not valid for this dialect.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="lineTerminator" /> is <see langword="null" />.</exception>
    public TabularDialect(string lineTerminator, char delimiter, char quoteChar)
        : this(lineTerminator, delimiter, quoteChar, quoteChar, null)
    {
    }

    /// <summary>Gets the character sequence that is used to separate records.</summary>
    /// <value>A string that has one or two UTF-16 code units.</value>
    public string LineTerminator
    {
        get;
    }

    /// <summary>Gets the character that is used to separate fields.</summary>
    /// <value>A UTF-16 code unit.</value>
    public char Delimiter
    {
        get;
    }

    /// <summary>Gets the character that is used to surround an escaped field.</summary>
    /// <value>A UTF-16 code unit.</value>
    public char QuoteChar
    {
        get;
    }

    /// <summary>Gets the character that is used to escape the quote character and itself within an escaped field.</summary>
    /// <value>A UTF-16 code unit. The same as <see cref="QuoteChar" /> if the dialect has no separate escape symbol.</value>
    public char EscapeChar
    {
        get;
    }

    /// <summary>Gets the character that is used at the beginning of a record to indicate that the record is a comment.</summary>
    /// <value>A UTF-16 code unit, or <see langword="null" /> if the dialect has no comment prefix.</value>
    public char? CommentPrefix
    {
        get;
    }
}

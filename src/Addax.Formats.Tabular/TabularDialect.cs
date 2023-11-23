// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Addax.Formats.Tabular;

/// <summary>Specifies how to read and write tabular data. This class cannot be inherited.</summary>
public sealed class TabularDialect
{
    internal readonly TabularSearchValues SearchValues;

    private TabularDialect(string lineTerminator, char delimiter, char quoteSymbol, char escapeSymbol, char? annotationPrefix)
    {
        ArgumentNullException.ThrowIfNull(lineTerminator);

        if (!IsSupportedLineTerminator(lineTerminator))
        {
            ThrowTokenException(nameof(lineTerminator));
        }

        if (lineTerminator.Contains(delimiter, StringComparison.Ordinal))
        {
            ThrowTokenException(nameof(delimiter));
        }

        if (lineTerminator.Contains(quoteSymbol, StringComparison.Ordinal) || (quoteSymbol == delimiter))
        {
            ThrowTokenException(nameof(quoteSymbol));
        }

        if (lineTerminator.Contains(escapeSymbol, StringComparison.Ordinal) || (escapeSymbol == delimiter))
        {
            ThrowTokenException(nameof(escapeSymbol));
        }

        if (annotationPrefix.HasValue)
        {
            var annotationPrefixValue = annotationPrefix.Value;

            if (lineTerminator.Contains(annotationPrefixValue, StringComparison.Ordinal) ||
                (annotationPrefixValue == delimiter) ||
                (annotationPrefixValue == quoteSymbol) ||
                (annotationPrefixValue == escapeSymbol))
            {
                ThrowTokenException(nameof(annotationPrefix));
            }
        }

        LineTerminator = lineTerminator;
        Delimiter = delimiter;
        QuoteSymbol = quoteSymbol;
        EscapeSymbol = escapeSymbol;
        AnnotationPrefix = annotationPrefix;

        SearchValues = new(lineTerminator, delimiter, quoteSymbol, escapeSymbol);

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowTokenException(string paramName)
        {
            throw new ArgumentException("The token value is not valid for this dialect.", paramName);
        }
    }

    /// <summary>Initializes a new instance of the <see cref="TabularDialect" /> class with specified tokens.</summary>
    /// <param name="lineTerminator">The character sequence that is used to separate records. Must have one or two distinct characters.</param>
    /// <param name="delimiter">The character that is used to separate fields. Must differ from line terminator characters.</param>
    /// <param name="quoteSymbol">The character that is used to surround an escaped field. Must differ from line terminator and delimiter characters.</param>
    /// <param name="escapeSymbol">The character that is used to escape the quote symbol and itself within an escaped field. Must differ from line terminator and delimiter characters.</param>
    /// <param name="annotationPrefix">The character that is used at the beginning of a record to indicate that the record is an annotation. Must differ from line terminator, delimiter, quote, and escape characters.</param>
    /// <exception cref="ArgumentException">A token value is not valid for this dialect.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="lineTerminator" /> is <see langword="null" />.</exception>
    public TabularDialect(string lineTerminator, char delimiter, char quoteSymbol, char escapeSymbol, char annotationPrefix)
        : this(lineTerminator, delimiter, quoteSymbol, escapeSymbol, (char?)annotationPrefix)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularDialect" /> class with specified tokens.</summary>
    /// <param name="lineTerminator">The character sequence that is used to separate records. Must have one or two distinct characters.</param>
    /// <param name="delimiter">The character that is used to separate fields. Must differ from line terminator characters.</param>
    /// <param name="quoteSymbol">The character that is used to surround an escaped field. Must differ from line terminator and delimiter characters.</param>
    /// <param name="escapeSymbol">The character that is used to escape the quote symbol and itself within an escaped field. Must differ from line terminator and delimiter characters.</param>
    /// <exception cref="ArgumentException">A token value is not valid for this dialect.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="lineTerminator" /> is <see langword="null" />.</exception>
    public TabularDialect(string lineTerminator, char delimiter, char quoteSymbol, char escapeSymbol)
        : this(lineTerminator, delimiter, quoteSymbol, escapeSymbol, null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularDialect" /> class with specified tokens.</summary>
    /// <param name="lineTerminator">The character sequence that is used to separate records. Must have one or two distinct characters.</param>
    /// <param name="delimiter">The character that is used to separate fields. Must differ from line terminator characters.</param>
    /// <param name="quoteSymbol">The character that is used to surround an escaped field and escape itself within an escaped field. Must differ from line terminator and delimiter characters.</param>
    /// <exception cref="ArgumentException">A token value is not valid for this dialect.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="lineTerminator" /> is <see langword="null" />.</exception>
    public TabularDialect(string lineTerminator, char delimiter, char quoteSymbol)
        : this(lineTerminator, delimiter, quoteSymbol, quoteSymbol, null)
    {
    }

    internal static bool IsSupportedLineTerminator(string value)
    {
        return value.Length switch
        {
            1 => true,
            2 => value[0] != value[1],
            _ => false,
        };
    }

    /// <summary>Gets the character sequence that is used to separate records.</summary>
    /// <value>A strings that has one or two UTF-16 code units.</value>
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
    public char QuoteSymbol
    {
        get;
    }

    /// <summary>Gets the character that is used to escape the quote symbol and itself within an escaped field.</summary>
    /// <value>A UTF-16 code unit. Equals to <see cref="QuoteSymbol" />, if not set.</value>
    public char EscapeSymbol
    {
        get;
    }

    /// <summary>Gets the character that is used at the beginning of a record to indicate that the record is an annotation.</summary>
    /// <value>A UTF-16 code unit or <see langword="null" />, if not set.</value>
    public char? AnnotationPrefix
    {
        get;
    }
}

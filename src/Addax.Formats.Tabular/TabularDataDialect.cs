// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Describes a tabular data dialect.</summary>
public sealed class TabularDataDialect
{
    private TabularDataDialect(string lineTerminator, char delimiter, char quoteChar, char escapeChar, char? commentPrefix)
    {
        ArgumentException.ThrowIfNullOrEmpty(lineTerminator);

        if (!TabularFormatInfo.IsUnicodeMandatoryBreak(lineTerminator))
        {
            throw new ArgumentException("The specified value cannot be used for this token.", nameof(lineTerminator));
        }
        if (TabularFormatInfo.IsUnicodeMandatoryBreak(delimiter))
        {
            throw new ArgumentException("The specified value cannot be used for this token.", nameof(delimiter));
        }
        if (TabularFormatInfo.IsUnicodeMandatoryBreak(quoteChar) ||
            (quoteChar == delimiter))
        {
            throw new ArgumentException("The specified value cannot be used for this token.", nameof(quoteChar));
        }
        if (TabularFormatInfo.IsUnicodeMandatoryBreak(escapeChar) ||
            (escapeChar == delimiter))
        {
            throw new ArgumentException("The specified value cannot be used for this token.", nameof(escapeChar));
        }

        if (commentPrefix is not null)
        {
            if (TabularFormatInfo.IsUnicodeMandatoryBreak(commentPrefix.Value) ||
                (commentPrefix.Value == delimiter) ||
                (commentPrefix.Value == quoteChar) ||
                (commentPrefix.Value == escapeChar))
            {
                throw new ArgumentException("The specified value cannot be used for this token.", nameof(commentPrefix));
            }
        }

        LineTerminator = lineTerminator;
        Delimiter = delimiter;
        QuoteChar = quoteChar;
        EscapeChar = escapeChar;
        CommentPrefix = commentPrefix;
    }

    /// <summary>Initializes a new instance of the <see cref="TabularDataDialect" /> class using the specified tokens.</summary>
    /// <param name="lineTerminator">The character sequence that is used to separate records, must be a hard line break (LF, VT, FF, CR, NL, LS, PS, or CR+LF).</param>
    /// <param name="delimiter">The character that is used to separate fields.</param>
    /// <param name="quoteChar">The character that is used to surround an escaped field.</param>
    /// <param name="escapeChar">The character that is used to escape the quote character and itself in an escaped field.</param>
    /// <param name="commentPrefix">The character that is used at the beginning of a line to indicate that the line is a comment.</param>
    /// <exception cref="ArgumentException">A token or the combination of tokens is not supported.</exception>
    public TabularDataDialect(string lineTerminator, char delimiter, char quoteChar, char escapeChar, char commentPrefix)
        : this(lineTerminator, delimiter, quoteChar, escapeChar, (char?)commentPrefix)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularDataDialect" /> class using the specified tokens.</summary>
    /// <param name="lineTerminator">The character sequence that is used to separate records, must be a hard line break (LF, VT, FF, CR, NL, LS, PS, or CR+LF).</param>
    /// <param name="delimiter">The character that is used to separate fields.</param>
    /// <param name="quoteChar">The character that is used to surround an escaped field.</param>
    /// <param name="escapeChar">The character that is used to escape the quote character and itself in an escaped field.</param>
    /// <exception cref="ArgumentException">A token or the combination of tokens is not supported.</exception>
    public TabularDataDialect(string lineTerminator, char delimiter, char quoteChar, char escapeChar)
        : this(lineTerminator, delimiter, quoteChar, escapeChar, null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularDataDialect" /> class using the specified tokens.</summary>
    /// <param name="lineTerminator">The character sequence that is used to separate records, must be a hard line break (LF, VT, FF, CR, NL, LS, PS, or CR+LF).</param>
    /// <param name="delimiter">The character that is used to separate fields.</param>
    /// <param name="quoteChar">The character that is used to surround an escaped field and to escape itself in an escaped field.</param>
    /// <exception cref="ArgumentException">A token or the combination of tokens is not supported.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="lineTerminator" /> is <see langword="null" />.</exception>
    public TabularDataDialect(string lineTerminator, char delimiter, char quoteChar)
        : this(lineTerminator, delimiter, quoteChar, quoteChar, null)
    {
    }

    /// <summary>Gets the character sequence that is used to separate records.</summary>
    /// <value>The provided <see cref="string" /> instance.</value>
    public string LineTerminator
    {
        get;
    }

    /// <summary>Gets the character that is used to separate fields.</summary>
    /// <value>The provided <see cref="char" /> value.</value>
    public char Delimiter
    {
        get;
    }

    /// <summary>Gets the character that is used to surround an escaped field.</summary>
    /// <value>The provided <see cref="char" /> value.</value>
    public char QuoteChar
    {
        get;
    }

    /// <summary>Gets the character that is used to escape the quote character and itself in an escaped field.</summary>
    /// <value>The provided <see cref="char" /> value.</value>
    public char EscapeChar
    {
        get;
    }

    /// <summary>Gets the character that is used at the beginning of a line to indicate that the line is a comment.</summary>
    /// <value>The provided <see cref="char" /> value or <see langword="null" />.</value>
    public char? CommentPrefix
    {
        get;
    }
}

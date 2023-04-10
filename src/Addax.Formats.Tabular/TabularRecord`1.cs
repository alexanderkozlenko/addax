// (c) Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CA1000

using System.Diagnostics.CodeAnalysis;

namespace Addax.Formats.Tabular;

/// <summary>Represents a record that contains data or a comment.</summary>
/// <typeparam name="T">The type of record data.</typeparam>
public readonly struct TabularRecord<T> : IEquatable<TabularRecord<T>>
{
    private readonly T? _content;
    private readonly string? _comment;
    private readonly bool _hasContent;

    private TabularRecord(bool hasContent, T? content, string? comment)
    {
        _hasContent = hasContent;
        _content = content;
        _comment = comment;
    }

    /// <summary>Creates a new record with the specified value as data.</summary>
    /// <param name="value">The record data.</param>
    public static TabularRecord<T> AsContent(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return new(hasContent: true, content: value, comment: null);
    }

    /// <summary>Creates a new record with the specified value as a comment.</summary>
    /// <param name="value">The record comment.</param>
    public static TabularRecord<T> AsComment(string? value)
    {
        return new(hasContent: false, content: default, comment: value);
    }

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return (obj is TabularRecord<T> other) && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(_hasContent, _content, _comment);
    }

    /// <inheritdoc />
    public bool Equals(TabularRecord<T> other)
    {
        return
            (_hasContent == other._hasContent) &&
            EqualityComparer<T>.Default.Equals(_content, other._content) &&
            StringComparer.Ordinal.Equals(_comment, other._comment);
    }

    /// <summary />
    public static bool operator ==(TabularRecord<T> left, TabularRecord<T> right)
    {
        return left.Equals(right);
    }

    /// <summary />
    public static bool operator !=(TabularRecord<T> left, TabularRecord<T> right)
    {
        return !left.Equals(right);
    }

    /// <summary> Gets a value indicating whether the current record contains data.</summary>
    /// <value><see langword="true" /> if the current record contains data; <see langword="false" /> otherwise.</value>
    [MemberNotNullWhen(true, nameof(Content))]
    public bool HasContent
    {
        get
        {
            return _hasContent;
        }
    }

    /// <summary>Gets the record data if the current record contains data.</summary>
    /// <value>An instance or the default value of <typeparamref name="T" />.</value>
    public T? Content
    {
        get
        {
            return _content;
        }
    }

    /// <summary>Gets the record comment if the current record represents a comment.</summary>
    /// <value>A <see cref="string" /> value or <see langword="null" />.</value>
    public string? Comment
    {
        get
        {
            return _comment;
        }
    }
}

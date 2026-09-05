using System.Text.RegularExpressions;
using ReserveFlow.Domain.Abstractions;

namespace ReserveFlow.Domain.Shared;

public sealed partial class Email : ValueObject
{
    private static readonly Regex EmailRegex = MyRegex();

    private Email(string value)
    {
        Value = value;
    }

    // EF Core materialization
    private Email()
    {
        Value = null!;
    }

    public string Value { get; private set; }

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<Email>(EmailError.Required);
        }

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length > 256 || !EmailRegex.IsMatch(normalized))
        {
            return Result.Failure<Email>(EmailError.InvalidFormat);
        }

        return new Email(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}

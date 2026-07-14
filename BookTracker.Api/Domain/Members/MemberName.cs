namespace BookTracker.Api.Domain.Members;

public record MemberName
{
    public const int MaxLength = 100;

    public string Value { get; }

    public MemberName(string value)
    {
        var cleaned = value.Trim();

        if (string.IsNullOrWhiteSpace(cleaned))
        {
            throw new DomainException("Name is required.");
        }

        if (cleaned.Length > MaxLength)
        {
            throw new DomainException($"Name cannot be longer than {MaxLength} characters.");
        }

        Value = cleaned;
    }
    public static implicit operator string(MemberName name)
    {
        return name.Value;
    }
    public override string ToString()
    {
        return Value;
    }
}
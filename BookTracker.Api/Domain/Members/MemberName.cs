namespace BookTracker.Api.Domain.Members;

public record MemberName
{
    public const int MaxLength = 100;

    public string Value { get; }

    public MemberName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Name is required.");
        }

        var cleaned = value.Trim();

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
using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.Domain.Members;

public class MemberNameTests
{
    [Fact]
    public void MemberNameAcceptsValidName()
    {
        var member = new MemberName("Lukas Motte");

        Assert.Equal("Lukas Motte", member.Value);
    }

    [Fact]
    public void MemberNameTrimsValue()
    {
        var member = new MemberName("   Lukas Motte   ");

        Assert.Equal("Lukas Motte", member.Value);
    }

    [Fact]
    public void MemberNameRejectsWhiteSpace()
    {
        var exception = Assert.Throws<DomainException>(() => new MemberName("    "));

        Assert.Equal("Name is required.", exception.Message);
    }

    [Fact]
    public void MemberNameRejectsNamesLongerThan100Characters()
    {
        var value = new string('x', 101);

        var exception = Assert.Throws<DomainException>(() => new MemberName(value));

        Assert.Equal("Name cannot be longer than 100 characters.", exception.Message);
    }
}
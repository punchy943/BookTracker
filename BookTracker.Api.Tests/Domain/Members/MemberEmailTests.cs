using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.Domain.Members;

public class MemberEmailTests
{
    [Fact]
    public void MemberEmailAcceptsValidEmail()
    {
        var email = new MemberEmail("lukasmotte75@gmail.com");

        Assert.Equal("lukasmotte75@gmail.com", email.Value);
    }

    [Fact]
    public void MemberEmailTrimsValue()
    {
        var email = new MemberEmail("   lukasmotte75@gmail.com   ");

        Assert.Equal("lukasmotte75@gmail.com", email.Value);
    }

    [Fact]
    public void MemberEmailRejectsWhiteSpace()
    {
        var exception = Assert.Throws<DomainException>(() => new MemberEmail("   "));

        Assert.Equal("Email is required.", exception.Message);
    }

    [Fact]
    public void MemberEmailRejectsEmailsLongerThan100Characters()
    {
        var value = new string('x', 201);

        var exception = Assert.Throws<DomainException>(() => new MemberEmail(value));

        Assert.Equal("Email cannot be longer than 200 characters.", exception.Message);
    }

    [Fact]
    public void MemberEmailRejectsEmailsNotContainingAt()
    {
        var exception = Assert.Throws<DomainException>(() => new MemberEmail("lukasmotte75gmail.com"));

        Assert.Equal("Email must contain '@'", exception.Message);
    }
}
using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.UpdateMember;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.UpdateMember;

public class UpdateMemberTests : IntegrationTest
{
    [Fact]
    public async Task PutMemberUpdatesExistingMember()
    {
        Writer.Seed(db =>
        {
            db.Members.Add(
                new Member
                {
                    Name = new MemberName("Lukas Motte"),
                    Email = new MemberEmail("lukasmotte75@gmail.com"),
                    PasswordHash = "test-password-hash"
                });
        });

        var request = 
            new UpdateMemberRequest
            {
                Name = "Peter-Paul",
                Email = "peterpaul@gmail.com"
            };

        var response = await Client.PutAsJsonAsync("/members/1", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var member = Reader.Query(db => db.Members.Find(1));

        Assert.NotNull(member);
        Assert.Equal("Peter-Paul", member.Name);
        Assert.Equal("peterpaul@gmail.com", member.Email);
    }

    [Fact]
    public async Task PutMemberReturnsNotFoundWithInvalidId()
    {
        var request = 
            new UpdateMemberRequest
            {
                Name = "Lukas Motte",
                Email = "lukasmotte75@gmail.com"
            };

        var response = await Client.PutAsJsonAsync("/members/1", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PutMemberReturnsBadRequestWhenEmailIsInvalid()
    {
        Writer.Seed(db =>
        {
            db.Members.Add(
                new Member
                {
                    Name = new MemberName("Lukas Motte"),
                    Email = new MemberEmail("lukasmotte75@gmail.com"),
                    PasswordHash = "test-password-hash"
                });
        });

        var request = 
            new UpdateMemberRequest
            {
                Name = "Peter-Paul",
                Email = "peterpaulgmail.com"
            };

        var response = await Client.PutAsJsonAsync("/members/1", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);

        var member = Reader.Query(db => db.Members.Find(1));

        Assert.NotNull(member);
        Assert.Equal("Lukas Motte", member.Name);
        Assert.Equal("lukasmotte75@gmail.com", member.Email);
    }

    [Fact]
    public async Task PutMemberReturnsConflictWhenEmailExist()
    {
        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member
                {
                    Name = new MemberName("Lukas Motte"),
                    Email = new MemberEmail("lukasmotte75@gmail.com"),
                    PasswordHash = "test-password-hash"
                },
                new Member
                {
                    Name = new MemberName("Joris Motte"),
                    Email = new MemberEmail("jorismotte@gmail.com"),
                    PasswordHash = "test-password-hash"
                });
        });

        var request = 
            new UpdateMemberRequest
            {
                Name = "Joris Motte",
                Email = "lukasmotte75@gmail.com"
            };

        var response = await Client.PutAsJsonAsync("/members/2", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Conflict);
    }
}
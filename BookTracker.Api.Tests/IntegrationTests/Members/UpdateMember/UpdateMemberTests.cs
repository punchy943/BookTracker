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
        var memberId = await AuthenticateAsMember();

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

        var response = await Client.PutAsJsonAsync($"/members/{memberId}", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var member = Reader.Query(db => db.Members.Find(1));

        Assert.NotNull(member);
        Assert.Equal("Peter-Paul", member.Name);
        Assert.Equal("peterpaul@gmail.com", member.Email);
    }

    [Fact]
    public async Task PutMemberReturnsForbiddenWithInvalidId()
    {
        await AuthenticateAsMember();

        var request =
            new UpdateMemberRequest
            {
                Name = "Lukas Motte",
                Email = "lukasmotte75@gmail.com"
            };

        var response = await Client.PutAsJsonAsync("/members/2", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task PutMemberReturnsBadRequestWhenEmailIsInvalid()
    {
        var memberId = await AuthenticateAsMember();

        var request =
            new UpdateMemberRequest
            {
                Name = "Peter-Paul",
                Email = "peterpaulgmail.com"
            };

        var response = await Client.PutAsJsonAsync($"/members/{memberId}", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);

        var member = Reader.Query(db => db.Members.Find(1));

        Assert.NotNull(member);
        Assert.Equal("Ada Lovelace", member.Name);
        Assert.Equal("ada@example.com", member.Email);
    }

    [Fact]
    public async Task PutMemberReturnsConflictWhenEmailExist()
    {
        var memberId = await AuthenticateAsMember();

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
                Name = "Joris Motte",
                Email = "lukasmotte75@gmail.com"
            };

        var response = await Client.PutAsJsonAsync($"/members/{memberId}", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Conflict);
    }
}
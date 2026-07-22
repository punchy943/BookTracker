using System.Net;
using BookTracker.Api.Application.Members.GetMemberDetails;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.GetMemberDetails;

public class MemberDetailsTests : IntegrationTest
{
    [Fact]
    public async Task GetMemberDetailsReturnsExistingMember()
    {
        await AuthenticateAsMember(
            MemberRole.Administrator);

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

        var response = await Client.GetAsync("/members/2");

        var member = await response.ReadJsonAs<GetMemberDetailsResponse>(HttpStatusCode.OK);

        Assert.NotNull(member);
        Assert.Equal("Lukas Motte", member.Name);
        Assert.Equal("lukasmotte75@gmail.com", member.Email);
    }

    [Fact]
    public async Task GetMemberReturnsNotFoundWhenIdDoesNotExist()
    {
        await AuthenticateAsMember(
                    MemberRole.Administrator);

        var response = await Client.GetAsync("/members/2");

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }
}
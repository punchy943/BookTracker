using System.Net;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.DeleteMember;

public class DeleteMemberTests : IntegrationTest
{
    [Fact]
    public async Task DeleteMemberDeletesMember()
    {
        Writer.Seed(db =>
        {
            db.Members.Add(
                new Member
                {
                    Name = new MemberName("Lukas Motte"),
                    Email = new MemberEmail("lukasmotte75@gmail.com")
                });
        });

        var response = await Client.DeleteAsync("/members/1");

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var member = Reader.Query(db => db.Members.Find(1));

        Assert.Null(member);
    }

    [Fact]
    public async Task DeleteMemberReturnsNotFoundWhenMemberDoesNotExist()
    {
        var response = await Client.DeleteAsync("/members/1");

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }
}
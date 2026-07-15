using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.CreateMember;

public class CreateMemberTests : IntegrationTest
{
    [Fact]
    public async Task PostMemberCreatesMember()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "Lukas Motte",
                Email = "lukasmotte75@gmail.com"
            };

        var response = await Client.PostAsJsonAsync("/members", request);

        var result = await response.ReadJsonAs<CreateMemberResponse>(HttpStatusCode.Created);

        var member = Reader.Query(context => context.Find<Member>(result.Id));

        Assert.NotNull(member);
        Assert.Equal("Lukas Motte", member.Name.Value);
        Assert.Equal("lukasmotte75@gmail.com", member.Email.Value);
    }

    [Fact]
    public async Task PostMembersReturnsBadRequestWhenNameIsWhiteSpace()
    {
        var request = 
            new CreateMemberResponse
            {
                Name = "    ",
                Email = "lukasmotte75@gmail.com"
            };

        var response = await Client.PostAsJsonAsync("/members", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostMembersReturnsBadRequestWhenNameIsInvalid()
    {
        var request = 
            new CreateMemberResponse
            {
                Name = "Lukas Motte",
                Email = "lukasmotte75gmail.com"
            };

        var response = await Client.PostAsJsonAsync("/members", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }
}
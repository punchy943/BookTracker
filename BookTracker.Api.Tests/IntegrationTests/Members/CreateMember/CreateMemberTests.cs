using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Domain.Members;
using Microsoft.AspNetCore.Identity;

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
                Email = "lukasmotte75@gmail.com",
                Password = "analytical-engine"
            };

        var response = await Client.PostAsJsonAsync("/members", request);

        var created = await response.ReadJsonAs<CreateMemberResponse>(HttpStatusCode.Created);

        // var member = Reader.Query(db => db.Find<Member>(created.Id));
        var member = Reader.Query(db =>
             db.Members.Single(current => current.Id == created.Id));

        Assert.NotEqual("analytical-engine", member?.PasswordHash);

        var passwordHasher = new PasswordHasher<Member>();

        var result = passwordHasher.VerifyHashedPassword(
            member!,
            member!.PasswordHash,
            "analytical-engine");

        Assert.Equal(PasswordVerificationResult.Success, result);

        Assert.NotNull(member);
        Assert.Equal("Lukas Motte", member.Name.Value);
        Assert.Equal("lukasmotte75@gmail.com", member.Email.Value);
    }

    [Fact]
    public async Task PostMembersReturnsBadRequestWhenNameIsWhiteSpace()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "    ",
                Email = "lukasmotte75@gmail.com",
                Password = "analytical-engine"
            };

        var response = await Client.PostAsJsonAsync("/members", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostMembersReturnsBadRequestWhenNameIsInvalid()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "Lukas Motte",
                Email = "lukasmotte75gmail.com",
                Password = "analytical-engine"
            };

        var response = await Client.PostAsJsonAsync("/members", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostMembersReturnsBadRequestWhenPasswordIsEmpty()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "Lukas Motte",
                Email = "lukasmotte75gmail.com",
                Password = ""
            };

        var response = await Client.PostAsJsonAsync("/members", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostMembersReturnsBadRequestWhenPasswordIsTooShort()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "Lukas Motte",
                Email = "lukasmotte75gmail.com",
                Password = "aabb"
            };

        var response = await Client.PostAsJsonAsync("/members", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostMembersReturnsConflictWhenEmailExists()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "Lukas Motte",
                Email = "lukasmotte75@gmail.com",
                Password = "analytical-engine"
            };

        await Client.PostAsJsonAsync("/members", request);

        var response = await Client.PostAsJsonAsync("/members", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateMemberCreatesRegularMember()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "Grace Hopper",
                Email = "grace@example.com",
                Password = "debugging-moth"
            };

        var response =
            await Client.PostAsJsonAsync(
                "/members",
                request);

        var created =
            await response
                .ReadJsonAs<CreateMemberResponse>(
                    HttpStatusCode.Created);

        var member =
            Reader.Query(db =>
                db.Members.Find(created.Id));

        Assert.NotNull(member);

        Assert.Equal(
            MemberRole.Member,
            member.Role);
    }
}
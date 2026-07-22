using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Application.Members.UpdateMember;
using BookTracker.Api.Domain.Members;
using Microsoft.AspNetCore.Identity;

namespace BookTracker.Api.Tests.IntegrationTests.Members.Authorization;

public class MemberAuthorizationTests : IntegrationTest
{
    private int SeedMemberWithId(string password = "analytical-engine")
    {
        var member =
            new Member
            {
                Name = new MemberName("Lukas Motte"),
                Email = new MemberEmail("lukasmotte75@gmail.com"),
                PasswordHash = string.Empty
            };

        var passwordHasher = new PasswordHasher<Member>();

        member.PasswordHash =
            passwordHasher.HashPassword(member, password);

        Writer.Seed(db => db.Members.Add(member));

        return member.Id;
    }

    [Fact]
    public async Task CreateMemberDoesNotRequireAuthentication()
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

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Created);
    }

    [Fact]
    public async Task UpdateMemberRequiresAuthentication()
    {
        var memberId = SeedMemberWithId();

        var request =
            new UpdateMemberRequest
            {
                Name = "Ada Byron",
                Email = "ada.byron@example.com"
            };

        var response =
            await Client.PutAsJsonAsync(
                $"/members/{memberId}",
                request);

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteMemberRequiresAuthorization()
    {
        var memberId = SeedMemberWithId();

        var response =
            await Client.DeleteAsync($"/members/{memberId}");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);

        var member = Reader.Query(db => db.Members.Find(1));

        Assert.NotNull(member);
    }

    [Fact]
    public async Task MemberCanUpdateOwnAccount()
    {
        var memberId = await AuthenticateAsMember();

        var request =
            new UpdateMemberRequest
            {
                Name = "Ada Byron",
                Email = "ada.byron@example.com"
            };

        var response =
            await Client.PutAsJsonAsync(
                $"/members/{memberId}",
                request);

        await response.ShouldHaveStatusCode(
            HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task MemberCannotUpdateOtherMember()
    {
        await AuthenticateAsMember();

        var memberId = SeedMemberWithId();

        var request =
            new UpdateMemberRequest
            {
                Name = "Changed Name",
                Email = "changed@example.com"
            };

        var response =
            await Client.PutAsJsonAsync(
                $"/members/{memberId}",
                request);

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Forbidden);

        var member =
        Reader.Query(db =>
            db.Members.Find(memberId));

        Assert.NotNull(member);
        Assert.Equal("Lukas Motte", member.Name.Value);
        Assert.Equal("lukasmotte75@gmail.com", member.Email.Value);
    }

    [Fact]
    public async Task MemberCannotDeleteOtherMember()
    {
        await AuthenticateAsMember();

        var memberId = SeedMemberWithId();

        var response =
            await Client.DeleteAsync($"/members/{memberId}");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Forbidden);

        var member = Reader.Query(db => db.Members.Find(2));

        Assert.NotNull(member);
    }
}
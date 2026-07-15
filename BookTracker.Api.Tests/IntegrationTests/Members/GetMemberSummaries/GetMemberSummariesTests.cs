using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.GetMemberSummaries;

public class GetMemberSummariesTests : IntegrationTest
{
    [Fact]
    public async Task GetMemberSummariesReturnsMembers()
    {
        Writer.Seed(db => db.Members.Add(
            new Member
            {
                Name = new MemberName("Lukas Motte"),
                Email = new MemberEmail("lukasmotte75@gmail.com")
            }
        ));

        var response = await Client.GetAsync("/members");

        var result = await response.ReadJsonAs<GetMemberSummariesResponse>(HttpStatusCode.OK);

        Assert.NotNull(result);

        var memberSummary = Assert.Single(result.Items);

        Assert.Equal("Lukas Motte", memberSummary.Name);
        Assert.Equal("lukasmotte75@gmail.com", memberSummary.Email);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesCanSearchByName()
    {
        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member
                {
                    Name = new MemberName("Lukas Motte"),
                    Email = new MemberEmail("lukasmotte75@gmail.com"),
                },
                new Member
                {
                    Name = new MemberName("Peter-Paul"),
                    Email = new MemberEmail("peterpaul@gmail.com"),
                });
        });

        var response = await Client.GetAsync("/members?search=Lukas");

        var result = await response.ReadJsonAs<GetMemberSummariesResponse>(HttpStatusCode.OK);

        var member = Assert.Single(result.Items);

        Assert.Equal("Lukas Motte", member.Name);
        Assert.Equal("lukasmotte75@gmail.com", member.Email);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesCanSearchByEmail()
    {
        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member
                {
                    Name = new MemberName("Lukas Motte"),
                    Email = new MemberEmail("lukasmotte75@gmail.com"),
                },
                new Member
                {
                    Name = new MemberName("Peter-Paul"),
                    Email = new MemberEmail("peterpaul@gmail.com"),
                });
        });

        var response = await Client.GetAsync("/members?search=lukasmotte75@gmail.com");

        var result = await response.ReadJsonAs<GetMemberSummariesResponse>(HttpStatusCode.OK);

        var member = Assert.Single(result.Items);

        Assert.Equal("Lukas Motte", member.Name);
        Assert.Equal("lukasmotte75@gmail.com", member.Email);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }


    [Fact]
    public async Task GetMemberSummariesAppliesPagingAfterSearch()
    {
        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member
                {
                    Name = new MemberName("Lukas Motte"),
                    Email = new MemberEmail("lukasmotte75@gmail.com")
                },
                new Member
                {
                    Name = new MemberName("Joris Motte"),
                    Email = new MemberEmail("jorismotte@gmail.com")
                },
                new Member
                {
                    Name = new MemberName("Peter-Paul"),
                    Email = new MemberEmail("peterpaul@gmail.com")
                });
        });

        var response = await Client.GetAsync("/members?search=Motte&page=2&pageSize=1");

        var result = await response.ReadJsonAs<GetMemberSummariesResponse>(HttpStatusCode.OK);

        var member = Assert.Single(result.Items);

        Assert.Equal("Joris Motte", member.Name);
        Assert.Equal(2, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.TotalPages);
    }

}

using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Actors;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.Domain.Members;

public class MmemberPermissionsTests
{
    [Fact]
    public void AdministratorCanViewMembers()
    {
        var actor = new Actor(1, MemberRole.Administrator);

        MemberPermissions.EnsureCanViewDirectory(actor);
    }

    [Fact]
    public void AdministratorCanManageOtherMembers()
    {
        var actor = new Actor(1, MemberRole.Administrator);

        MemberPermissions.EnsureCanManage(actor, 2);
    }

    [Fact]
    public void MemberCannotViewMembers()
    {
        var actor = new Actor(1, MemberRole.Member);

        Assert.Throws<ForbiddenOperationException>(() => MemberPermissions.EnsureCanViewDirectory(actor));
    }

    [Fact]
    public void MemberCanManageOwnAccount()
    {
        var actor = new Actor(1, MemberRole.Member);

        MemberPermissions.EnsureCanManage(actor, 1);
    }

    [Fact]
    public void MemberCannotManageOtherMember()
    {
        var actor = new Actor(1, MemberRole.Member);

        Assert.Throws<ForbiddenOperationException>(() => MemberPermissions.EnsureCanManage(actor, 2));
    }
}
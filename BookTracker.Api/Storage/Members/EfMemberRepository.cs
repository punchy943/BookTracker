using BookTracker.Api.Domain.Members;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Storage.Members;

public class EfMemberRepository(AppDbContext dbContext) : IMemberRepository
{
    public async Task<Member> AddAsync(Member member)
    {
        dbContext.Members.Add(member);
        await dbContext.SaveChangesAsync();
        return member;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // var member = await dbContext.Members.FindAsync(id);

        // if (member is null)
        // {
        //     return false;
        // }

        // dbContext.Members.Remove(member);
        // await dbContext.SaveChangesAsync();
        // return true;

        return await dbContext.Members.Where(m => m.Id == id).ExecuteDeleteAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Member member)
    {
        var existingMember = await dbContext.Members.FindAsync(member.Id);

        if (existingMember is null)
        {
            return false;
        }

        existingMember.Name = member.Name;
        existingMember.Email = member.Email;

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EmailExistsAsync(MemberEmail email, int? memberIdToIgnore = null)
    {
        // var memberSameEmail = await dbContext.Members.Where(m => m.Email == email).FirstOrDefaultAsync();
        
        // if (memberSameEmail is null)
        // {
        //     return false;   
        // } 
        
        // if (memberSameEmail?.Id == memberIdToIgnore)
        // {
        //     return false; 
        // }

        // return true;

        return await dbContext.Members.AnyAsync(m => m.Email == email && (memberIdToIgnore == null || m.Id != memberIdToIgnore)); 
    }
}
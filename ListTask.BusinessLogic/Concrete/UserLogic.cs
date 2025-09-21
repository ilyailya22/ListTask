using ListTask.BusinessLogic.Abstract;
using ListTask.Core.Model;
using ListTask.Data.Abstract;
using Microsoft.EntityFrameworkCore;

namespace ListTask.BusinessLogic.Concrete;

public sealed class UserLogic(IRepository<User> userRepository) : IUserLogic
{
    public async Task<int> GetUserIdByUniqueIdAsync(Guid? uniqueId)
    {
        if (uniqueId == null)
        {
            throw new ArgumentNullException(nameof(uniqueId));
        }

        var userId = await userRepository.GetAll()
            .Where(x => x.UniqueId == uniqueId)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync();

        if (userId == null)
        {
            throw new Exception($"User with unique id {uniqueId.Value} not found");
        }

        return userId.Value;
    }
    
    public void CreateUser(string name)
    {
        var user = new User
        {
            Name = name
        };
        
        userRepository.Add(user);
    }

    public async Task<User[]> GetUsersAsync(
        int? take = null, 
        int? skip = null,
        int[] userIds = null)
    {
        var query = userRepository.GetAll();

        if (take.HasValue && skip.HasValue)
        {
            query = query.Skip(skip!.Value).Take(take!.Value);
        }

        if (userIds is { Length: > 0 })
        {
            query = query.Where(x => userIds.Contains(x.Id));
        }

        return await query.ToArrayAsync();
    }
}
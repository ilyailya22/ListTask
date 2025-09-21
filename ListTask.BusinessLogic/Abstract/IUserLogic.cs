using ListTask.Core.Model;

namespace ListTask.BusinessLogic.Abstract;

public interface IUserLogic
{
    Task<int> GetUserIdByUniqueIdAsync(Guid? uniqueId);
    void CreateUser(string name);

    Task<User[]> GetUsersAsync(
        int? take = null,
        int? skip = null,
        int[] userIds = null);
}
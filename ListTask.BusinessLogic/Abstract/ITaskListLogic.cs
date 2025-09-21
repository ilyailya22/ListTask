using ListTask.Core.Model;

namespace ListTask.BusinessLogic.Abstract;

public interface ITaskListLogic
{
    Task<TaskList[]> GetTaskListsByUserIdAsync(int userId, int take, int skip);
    Task<TaskList> GetTaskListByUniqueIdAsync(Guid taskListUniqueId, int userId);
    void CreateTaskList(int userId, string name);
    Task UpdateTaskListAsync(int userId, Guid taskListUniqueId, string name);
    Task DeleteTaskListAsync(int userId, Guid taskListUniqueId);
    Task ShareTaskListAsync(int currentUserId, Guid taskListUniqueId, int userId);
    Task DeleteShareTaskListAsync(int currentUserId, Guid taskListUniqueId, int userId);
    Task<int[]> GetTaskListSharedUsersAsync(int userId, Guid taskListUniqueId);
}
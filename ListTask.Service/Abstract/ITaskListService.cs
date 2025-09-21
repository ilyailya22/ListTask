using ListTask.WebApi.Model;

namespace ListTask.Service.Abstract;

public interface ITaskListService
{
    Task<TaskListsByUserIdResponse> GetTaskListsByUserIdAsync(TaskListsByUserIdRequest request);
    Task<TaskListByUniqueIdResponse> GetTaskListByUniqueIdAsync(TaskListByUniqueIdRequest request);
    Task CreateTaskListAsync(CreateTaskListRequest request);
    Task UpdateTaskListAsync(UpdateTaskListRequest request);
    Task DeleteTaskListAsync(DeleteTaskListRequest request);
    Task ShareTaskListAsync(ShareTaskListRequest request);
    Task DeleteShareTaskListAsync(DeleteShareTaskListRequest request);
    Task<TaskListSharedUsersResponse> GetTaskListSharedUsersAsync(TaskListSharedUsersRequest request);
}
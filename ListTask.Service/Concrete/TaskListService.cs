using ListTask.BusinessLogic.Abstract;
using ListTask.Data.Abstract;
using ListTask.Service.Abstract;
using ListTask.WebApi.Model;

namespace ListTask.Service.Concrete;

public sealed class TaskListService(
    ITaskListLogic taskListLogic, 
    IUserLogic userLogic, 
    IUnitOfWork unitOfWork) : ITaskListService
{
    public async Task<TaskListsByUserIdResponse> GetTaskListsByUserIdAsync(TaskListsByUserIdRequest request)
    {
        var userId = await userLogic.GetUserIdByUniqueIdAsync(request.UserUniqueId);
        
        var taskListLists = await taskListLogic.GetTaskListsByUserIdAsync(userId, request.Take!.Value, request.Skip!.Value);

        return new TaskListsByUserIdResponse
        {
            TaskLists = taskListLists.Select(x => new TaskListInfo
            {
                Name = x.Name,
                UniqueId = x.UniqueId
            }).ToArray()
        };
    }

    public async Task<TaskListByUniqueIdResponse> GetTaskListByUniqueIdAsync(TaskListByUniqueIdRequest request)
    {
        var userId = await userLogic.GetUserIdByUniqueIdAsync(request.UserUniqueId);
        
        var taskList = await taskListLogic.GetTaskListByUniqueIdAsync(request.TaskListUniqueId!.Value, userId);

        return new TaskListByUniqueIdResponse
        {
            TaskList = new TaskListInfo
            {
                UniqueId = taskList.UniqueId,
                Name = taskList.Name
            }
        };
    }

    public async Task CreateTaskListAsync(CreateTaskListRequest request)
    {
        var userId = await userLogic.GetUserIdByUniqueIdAsync(request.UserUniqueId);
        
        taskListLogic.CreateTaskList(userId, request.Name);

        await unitOfWork.CommitAsync();
    }
    
    public async Task UpdateTaskListAsync(UpdateTaskListRequest request)
    {
        var userId = await userLogic.GetUserIdByUniqueIdAsync(request.UserUniqueId);
        
        await taskListLogic.UpdateTaskListAsync(userId, request.TaskListUniqueId!.Value, request.Name);

        await unitOfWork.CommitAsync();
    }
    
    public async Task DeleteTaskListAsync(DeleteTaskListRequest request)
    {
        var userId = await userLogic.GetUserIdByUniqueIdAsync(request.UserUniqueId);
        
        await taskListLogic.DeleteTaskListAsync(userId, request.TaskListUniqueId!.Value);

        await unitOfWork.CommitAsync();
    }
    
    public async Task ShareTaskListAsync(ShareTaskListRequest request)
    {
        var currentUserId = await userLogic.GetUserIdByUniqueIdAsync(request.CurrentUserUniqueId);
        var userId = await userLogic.GetUserIdByUniqueIdAsync(request.UserUniqueId);
        
        await taskListLogic.ShareTaskListAsync(currentUserId, request.TaskListUniqueId!.Value, userId);

        await unitOfWork.CommitAsync();
    }
    
    public async Task DeleteShareTaskListAsync(DeleteShareTaskListRequest request)
    {
        var currentUserId = await userLogic.GetUserIdByUniqueIdAsync(request.CurrentUserUniqueId);
        var userId = await userLogic.GetUserIdByUniqueIdAsync(request.UserUniqueId);
        
        await taskListLogic.DeleteShareTaskListAsync(currentUserId, request.TaskListUniqueId!.Value, userId);

        await unitOfWork.CommitAsync();
    }

    public async Task<TaskListSharedUsersResponse> GetTaskListSharedUsersAsync(TaskListSharedUsersRequest request)
    {
        var userId = await userLogic.GetUserIdByUniqueIdAsync(request.UserUniqueId);
        
        var sharedUsers = await taskListLogic.GetTaskListSharedUsersAsync(userId, request.TaskListUniqueId!.Value);

        var sharedUserInfos = await userLogic.GetUsersAsync(userIds: sharedUsers);

        return new TaskListSharedUsersResponse
        {
            TaskListSharedUsers = sharedUserInfos.Select(x => new UserInfo
            {
                UniqueId = x.UniqueId,
                Name = x.Name
            }).ToArray()
        };
    }
}
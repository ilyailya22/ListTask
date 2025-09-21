using ListTask.Service.Abstract;
using ListTask.WebApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace ListTask.WebApi.Controllers;

[ApiController]
[Route("api/tasklists")]
public sealed class TaskListController(ITaskListService taskListService) : Controller
{
    [HttpGet("byuserid")]
    public Task<TaskListsByUserIdResponse> GetTaskListListsByUserId([FromQuery] TaskListsByUserIdRequest request)
    {
        return taskListService.GetTaskListsByUserIdAsync(request);
    }
    
    [HttpGet("byuniqueid")]
    public Task<TaskListByUniqueIdResponse> GetTaskListByUniqueId([FromQuery] TaskListByUniqueIdRequest request)
    {
        return taskListService.GetTaskListByUniqueIdAsync(request);
    }
    
    [HttpPost("create")]
    public Task CreateTaskList([FromBody] CreateTaskListRequest request)
    {
        return taskListService.CreateTaskListAsync(request);
    }
    
    [HttpPost("update")]
    public Task UpdateTaskList([FromBody] UpdateTaskListRequest request)
    {
        return taskListService.UpdateTaskListAsync(request);
    }
    
    [HttpPost("delete")]
    public Task DeleteTaskList([FromBody] DeleteTaskListRequest request)
    {
        return taskListService.DeleteTaskListAsync(request);
    }
    
    [HttpPost("share")]
    public Task ShareTaskList([FromBody] ShareTaskListRequest request)
    {
        return taskListService.ShareTaskListAsync(request);
    }
    
    [HttpPost("share/delete")]
    public Task DeleteShareTaskList([FromBody] DeleteShareTaskListRequest request)
    {
        return taskListService.DeleteShareTaskListAsync(request);
    }
            
    [HttpGet("share/users")]
    public Task<TaskListSharedUsersResponse> GetTaskListSharedUsers([FromQuery] TaskListSharedUsersRequest request)
    {
        return taskListService.GetTaskListSharedUsersAsync(request);
    }
}
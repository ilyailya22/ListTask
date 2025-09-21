using ListTask.Service.Abstract;
using ListTask.WebApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace ListTask.WebApi.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UserController(IUserService userService) : Controller
{
    [HttpGet]
    public Task<UsersResponse> GetTaskListByUniqueId([FromQuery] UsersRequest request)
    {
        return userService.GetUsersAsync(request);
    }
    
    [HttpPost("create")]
    public Task CreateUser([FromBody] CreateUserRequest request)
    {
        return userService.CreateUserAsync(request);
    }
}
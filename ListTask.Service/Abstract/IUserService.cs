using ListTask.WebApi.Model;

namespace ListTask.Service.Abstract;

public interface IUserService
{
    Task CreateUserAsync(CreateUserRequest request);
    Task<UsersResponse> GetUsersAsync(UsersRequest request);
}
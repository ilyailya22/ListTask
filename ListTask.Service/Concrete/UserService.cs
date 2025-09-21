using ListTask.BusinessLogic.Abstract;
using ListTask.Data.Abstract;
using ListTask.Service.Abstract;
using ListTask.WebApi.Model;

namespace ListTask.Service.Concrete;

public sealed class UserService(IUserLogic userLogic, IUnitOfWork unitOfWork) : IUserService
{
    public async Task CreateUserAsync(CreateUserRequest request)
    {
        userLogic.CreateUser(request.Name);
        await unitOfWork.CommitAsync();
    }

    public async Task<UsersResponse> GetUsersAsync(UsersRequest request)
    {
        var users = await userLogic.GetUsersAsync(request.Take, request.Skip);

        return new UsersResponse
        {
            Users = users.Select(x => new UserInfo
            {
                Name = x.Name,
                UniqueId = x.UniqueId
            }).ToArray()
        };
    }
}
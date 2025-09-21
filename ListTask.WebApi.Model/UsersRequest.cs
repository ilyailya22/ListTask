namespace ListTask.WebApi.Model;

public sealed class UsersRequest
{
    public int? Take { get; set; }
    public int? Skip { get; set; }
}
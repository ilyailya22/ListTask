namespace ListTask.WebApi.Model;

public sealed class TaskListSharedUsersRequest
{
    public Guid? UserUniqueId { get; set; }
    public Guid? TaskListUniqueId { get; set; }
}
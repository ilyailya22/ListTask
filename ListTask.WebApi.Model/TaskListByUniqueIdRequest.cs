namespace ListTask.WebApi.Model;

public sealed class TaskListByUniqueIdRequest
{
    public Guid? UserUniqueId { get; set; }
    public Guid? TaskListUniqueId { get; set; }
}
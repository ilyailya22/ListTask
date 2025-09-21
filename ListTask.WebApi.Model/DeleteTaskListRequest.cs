namespace ListTask.WebApi.Model;

public sealed class DeleteTaskListRequest
{
    public Guid? UserUniqueId { get; set; }
    public Guid? TaskListUniqueId { get; set; }
}
namespace ListTask.WebApi.Model;

public sealed class UpdateTaskListRequest
{
    public Guid? UserUniqueId { get; set; }
    public Guid? TaskListUniqueId { get; set; }
    public string Name { get; set; }
}
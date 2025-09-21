namespace ListTask.WebApi.Model;

public sealed class DeleteShareTaskListRequest
{
    public Guid? CurrentUserUniqueId { get; set; }
    public Guid? UserUniqueId { get; set; }
    public Guid? TaskListUniqueId { get; set; }
}
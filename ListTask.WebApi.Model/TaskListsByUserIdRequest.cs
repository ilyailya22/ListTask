namespace ListTask.WebApi.Model;

public sealed class TaskListsByUserIdRequest
{
    public Guid? UserUniqueId { get; set; }
    public int? Skip { get; set; }
    public int? Take { get; set; }
}
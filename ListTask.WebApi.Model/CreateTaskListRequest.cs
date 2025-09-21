namespace ListTask.WebApi.Model;

public sealed class CreateTaskListRequest
{
    public Guid? UserUniqueId { get; set; }
    public string Name { get; set; }
}
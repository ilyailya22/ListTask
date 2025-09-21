namespace ListTask.WebApi.Model;

public sealed class TaskListsByUserIdResponse
{
    public TaskListInfo[] TaskLists { get; set; }
}
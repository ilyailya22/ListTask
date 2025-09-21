namespace ListTask.Core.Model;

public sealed class TaskListShare
{
    public int Id { get; set; }

    public Guid UniqueId { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int TaskListId { get; set; }
    public TaskList TaskList { get; set; }
}
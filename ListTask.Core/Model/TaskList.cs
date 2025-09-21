namespace ListTask.Core.Model;

public sealed class TaskList
{
    public int Id { get; set; }

    public Guid UniqueId { get; set; }
    
    public DateTime Created { get; set; }

    public string Name { get; set; }
    
    public DateTime? Deleted { get; set; }
    
    public int OwnerId { get; set; }
    public User Owner { get; set; }
    
    public ICollection<TaskListShare> Shares { get; set; }
}
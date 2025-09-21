namespace ListTask.Core.Model;

public sealed class User
{
    public int Id { get; set; }

    public Guid UniqueId { get; set; }

    public string Name { get; set; }

    public ICollection<TaskList> OwnedTaskLists { get; set; }

    public ICollection<TaskListShare> SharedTaskLists { get; set; }
}
using ListTask.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListTask.Data.Mapping;

public sealed class TaskListShareMapper : IEntityTypeConfiguration<TaskListShare>
{
    public void Configure(EntityTypeBuilder<TaskListShare> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.UniqueId)
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd()
            .IsRequired();
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.SharedTaskLists)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.TaskList)
            .WithMany(x => x.Shares)
            .HasForeignKey(x => x.TaskListId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(x => x.UniqueId).IsUnique();
        builder.HasIndex(x => new { x.TaskListId, x.UserId }).IsUnique();
    }
}
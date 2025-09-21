using ListTask.Core.Const;
using ListTask.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListTask.Data.Mapping;

public sealed class TaskListMapper : IEntityTypeConfiguration<TaskList>
{
    public void Configure(EntityTypeBuilder<TaskList> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.UniqueId)
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd()
            .IsRequired();
        
        builder.Property(x => x.Name)
            .HasMaxLength(ListTaskConst.StringMaxLength)
            .IsRequired();
        
        builder.Property(x => x.Created)
            .HasDefaultValueSql("now() at time zone 'utc'")
            .ValueGeneratedOnAdd()
            .IsRequired();
        
        builder.HasOne(x => x.Owner)
            .WithMany(x => x.OwnedTaskLists)
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(x => x.UniqueId).IsUnique();
        builder.HasIndex(x => x.Created).IsUnique();
    }
}
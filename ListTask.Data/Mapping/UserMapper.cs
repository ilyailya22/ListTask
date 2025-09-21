using ListTask.Core.Const;
using ListTask.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListTask.Data.Mapping;

public sealed class UserMapper : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.UniqueId)
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd()
            .IsRequired();
        
        builder.Property(x => x.Name)
            .HasMaxLength(ListTaskConst.StringMaxLength)
            .IsRequired();
        
        builder.HasIndex(x => x.UniqueId).IsUnique();
    }
}
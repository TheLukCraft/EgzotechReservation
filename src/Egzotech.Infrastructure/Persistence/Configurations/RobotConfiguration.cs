using Egzotech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egzotech.Infrastructure.Persistence.Configurations;

internal class RobotConfiguration : IEntityTypeConfiguration<Robot>
{
    public void Configure(EntityTypeBuilder<Robot> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Model)
            .IsRequired()
            .HasMaxLength(100);
    }
}
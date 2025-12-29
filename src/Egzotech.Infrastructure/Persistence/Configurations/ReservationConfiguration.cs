using Egzotech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egzotech.Infrastructure.Persistence.Configurations;

internal class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.PatientEmail)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(x => x.Robot)
            .WithMany()
            .HasForeignKey(x => x.RobotId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(x => new { x.RobotId, x.StartTime });
    }
}
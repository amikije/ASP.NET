using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TmsApi.Domain.Entities;

namespace TmsApi.Infrastructure.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        // Primary Key
        builder.HasKey(s => s.Id);

        // Name
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        // GPA
        builder.Property(s => s.GPA)
            .HasPrecision(3, 2);

        // IsActive
        builder.Property(s => s.IsActive)
            .HasDefaultValue(true);

        // Shadow Property
        builder.Property<DateTime>("LastUpdated");

       
builder.Property(s => s.IsDeleted)
    .HasDefaultValue(false);
    builder.HasQueryFilter(s => !s.IsDeleted);
        // Relationship
        builder.HasMany(s => s.Enrollments)
            .WithOne(e => e.Student)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
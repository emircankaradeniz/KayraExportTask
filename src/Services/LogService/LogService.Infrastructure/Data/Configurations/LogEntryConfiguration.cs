using LogService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogService.Infrastructure.Data.Configurations;

public sealed class LogEntryConfiguration : IEntityTypeConfiguration<LogEntry>
{
    public void Configure(EntityTypeBuilder<LogEntry> builder)
    {
        builder.ToTable("Logs");

        builder.HasKey(log => log.Id);

        builder.Property(log => log.ServiceName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(log => log.Level)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(log => log.Message)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(log => log.Exception)
            .HasColumnType("nvarchar(max)");

        builder.Property(log => log.TraceId)
            .HasMaxLength(200);

        builder.Property(log => log.Path)
            .HasMaxLength(1000);

        builder.Property(log => log.Method)
            .HasMaxLength(20);

        builder.Property(log => log.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(log => log.Level);
        builder.HasIndex(log => log.CreatedAtUtc);
        builder.HasIndex(log => log.ServiceName);
    }
}
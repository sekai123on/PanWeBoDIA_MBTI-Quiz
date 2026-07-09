using Microsoft.EntityFrameworkCore;
using MbtiApi.Domain.Entities;
using MbtiApi.Domain.Enums;

namespace MbtiApi.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Question>    Questions    => Set<Question>();
    public DbSet<TestSession> TestSessions => Set<TestSession>();
    public DbSet<Answer>      Answers      => Set<Answer>();
    public DbSet<TestResult>  TestResults  => Set<TestResult>();
    public DbSet<User>        Users        => Set<User>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // ── Question ─────────────────────────────────────────────────────
        mb.Entity<Question>(e =>
        {
            e.HasKey(q => q.Id);
            e.Property(q => q.Dimension)
             .HasConversion<string>()
             .HasMaxLength(2);
            e.Property(q => q.TextEn).HasMaxLength(500).IsRequired();
            e.Property(q => q.TextMy).HasMaxLength(500).IsRequired();
            e.Property(q => q.OptionAEn).HasMaxLength(200).IsRequired();
            e.Property(q => q.OptionBEn).HasMaxLength(200).IsRequired();
            e.Property(q => q.OptionAMy).HasMaxLength(200).IsRequired();
            e.Property(q => q.OptionBMy).HasMaxLength(200).IsRequired();
            e.HasIndex(q => q.OrderIndex).IsUnique();
        });

        // ── TestSession ───────────────────────────────────────────────────
        mb.Entity<TestSession>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Language).HasMaxLength(5).IsRequired();
            e.HasMany(s => s.Answers)
             .WithOne(a => a.Session)
             .HasForeignKey(a => a.SessionId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(s => s.Result)
             .WithOne(r => r.Session)
             .HasForeignKey<TestResult>(r => r.SessionId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Answer ────────────────────────────────────────────────────────
        mb.Entity<Answer>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.Choice)
             .HasConversion<string>()
             .HasMaxLength(1);
            e.HasIndex(a => new { a.SessionId, a.QuestionId }).IsUnique();
        });

        // ── TestResult ────────────────────────────────────────────────────
        mb.Entity<TestResult>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.MbtiType).HasMaxLength(4).IsRequired();
        });
    }
}

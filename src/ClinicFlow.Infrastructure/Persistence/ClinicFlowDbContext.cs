using ClinicFlow.Domain.Tenancy;
using ClinicFlow.Domain.Patients;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Persistence;

public class ClinicFlowDbContext : DbContext
{
    public ClinicFlowDbContext(DbContextOptions<ClinicFlowDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Patient> Patients => Set<Patient>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("Tenants");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(x => x.Code)
                .IsUnique();
        });
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("Patients");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.PhoneNumber)
                .HasMaxLength(30);

            entity.Property(x => x.Email)
                .HasMaxLength(200);

            entity.Property(x => x.Notes)
                .HasMaxLength(2000);

            entity.HasIndex(x => x.TenantId);
        });
    }
}
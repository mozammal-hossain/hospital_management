using Microsoft.EntityFrameworkCore;
public class PatientDbContext : DbContext
{
    public DbSet<Patient> Patients { get; set; }

    public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(e =>
        {
            e.ToTable("Patients");
            e.HasKey(p => p.Id);
            e.Property(p => p.FullName).HasMaxLength(200);
            e.Property(p => p.Email).HasMaxLength(256);
            e.Property(p => p.PhoneNumber).HasMaxLength(20);
            e.Property(p => p.DateOfBirth).HasColumnType("date");
            e.Property(p => p.Gender).HasMaxLength(10);
            e.Property(p => p.AdmittedAt).HasColumnType("date");
            e.Property(p => p.IsDischarged).HasColumnType("bit");
        });
    }
}
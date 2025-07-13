using Microsoft.EntityFrameworkCore;
using YourAppNamespace.Models;

namespace YourAppNamespace.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Symptom> Symptoms { get; set; }
        public DbSet<PatientSymptomRecord> PatientSymptomsRecord { get; set; }
        public DbSet<PatientImage> PatientImages { get; set; }
        public DbSet<DiagnosisRule> DiagnosisRules { get; set; }
        public DbSet<SyncQueue> SyncQueue { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Symptoms)
                .WithOne(ps => ps.Patient)
                .HasForeignKey(ps => ps.PatientId);

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Images)
                .WithOne(pi => pi.Patient)
                .HasForeignKey(pi => pi.PatientId);

            modelBuilder.Entity<PatientSymptomRecord>()
                .HasOne(ps => ps.Symptom)
                .WithMany()
                .HasForeignKey(ps => ps.SymptomId);

            // ✅ Optional: Indexes for faster lookup
            modelBuilder.Entity<SyncQueue>()
                .HasIndex(sq => new { sq.EntityName, sq.Synced });
        }
    }
}

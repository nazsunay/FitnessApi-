using FitnessApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitnessApi.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Programs> Programs { get; set; }
        public DbSet<UserProgram> UserPrograms { get; set; }
        public DbSet<ProgramHistory> ProgramHistories { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // UserProgram ara tablosu için Many-to-Many ilişki yapılandırması
            modelBuilder.Entity<UserProgram>()
            .HasKey(up => up.Id);  // Sadece Id Primary Key

            modelBuilder.Entity<UserProgram>()
             .HasOne(up => up.User)
             .WithMany(u => u.UserPrograms)// birden çok varlıkla ilişkisi var
             .HasForeignKey(up => up.UserId)//fk
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserProgram>()
                .HasOne(up => up.Programs)
                .WithMany(p => p.UserPrograms)// user programs ın birden çok varlıkla ilişkisi olabilir
                .HasForeignKey(up => up.ProgramId)  // Foreign Key 
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProgramHistory>()
               .HasOne(ph => ph.User)//bire çok ilişkisi var
               .WithMany(u => u.ProgramHistories)//birden çok varlıkla ilişkisi var
               .HasForeignKey(ph => ph.UserId)// fk
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProgramHistory>()
                .HasOne(ph => ph.Programs)//program ile program historynin bire çok ilişkisi var 
                .WithMany(p => p.ProgramHistories)
                .HasForeignKey(ph => ph.ProgramId)  //  FK 
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

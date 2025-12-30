using ProjectLab.PucCampinas.Laboratories.Model;
using Microsoft.EntityFrameworkCore;
using ProjectLab.PucCampinas.Reservations.Model;
using ProjectLab.PucCampinas.Users.Model;

namespace ProjectLab.PucCampinas.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Laboratory> Laboratories { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Laboratory)
                .WithMany() 
                .HasForeignKey(r => r.LaboratoryId);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations) 
                .HasForeignKey(r => r.UserId);

        }
    }
}

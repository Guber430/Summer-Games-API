using Microsoft.EntityFrameworkCore;
using SummerGames.Models;

namespace SummerGames.Data
{
    public class SummerGamesContext : DbContext
    {
        public SummerGamesContext(DbContextOptions<SummerGamesContext> options)
            : base(options)
        {

        }

        public DbSet<Sport> Sports { get; set; }
        public DbSet<Contingent> Contingents { get; set; }
        public DbSet<Athlete> Athletes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Prevent Cascade Delete from Athlete to Sport
            //so we are prevented from deleting a Sport with
            //Athletes assigned
            modelBuilder.Entity<Athlete>()
                .HasOne(a => a.Sport)
                .WithMany(a => a.Athletes)
                .HasForeignKey(a => a.SportID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from Athlete to Contingent
            //so we are prevented from deleting a Contingent with
            //Athletes assigned
            modelBuilder.Entity<Athlete>()
                .HasOne(a => a.Contingent)
                .WithMany(a => a.Athletes)
                .HasForeignKey(a => a.ContingentID)
                .OnDelete(DeleteBehavior.Restrict);

            //Athlete: Athlete code is unique
            modelBuilder.Entity<Athlete>()
                .HasIndex(a => a.AthleteCode)
                .IsUnique();

            //Contingent: Code is unique
            modelBuilder.Entity<Contingent>()
                .HasIndex(c => c.Code)
                .IsUnique();

            //Sport: Code is unique
            modelBuilder.Entity<Sport>()
                .HasIndex(s => s.Code)
                .IsUnique();
        }
        public DbSet<SummerGames.Models.AthleteDTO> AthleteDTO { get; set; } = default!;
        public DbSet<SummerGames.Models.ContingentDTO> ContingentDTO { get; set; } = default!;
        public DbSet<SummerGames.Models.SportDTO> SportDTO { get; set; } = default!;
    }
}

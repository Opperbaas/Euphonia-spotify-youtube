using Microsoft.EntityFrameworkCore;
using Euphonia.DataAccessLayer.Models;

namespace Euphonia.DataAccessLayer.Context
{
    /// <summary>
    /// Database context - Entity Framework Core
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets voor bestaande database tabellen
        public DbSet<Muziek> Muziek { get; set; }
        public DbSet<Profiel> Profielen { get; set; }
        public DbSet<Historiek> Historieken { get; set; }
        public DbSet<MuziekAnalyse> MuziekAnalyses { get; set; }
        public DbSet<Notificatie> Notificaties { get; set; }
        public DbSet<Statistiek> Statistieken { get; set; }
        public DbSet<Stemming> Stemmingen { get; set; }
        public DbSet<StemmingType> StemmingTypes { get; set; }
        public DbSet<StemmingMuziek> StemmingMuzieks { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Entity configuraties
            ConfigureBestaandeEntities(modelBuilder);
        }

        private void ConfigureBestaandeEntities(ModelBuilder modelBuilder)
        {
            // User configuratie
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Muziek configuratie
            modelBuilder.Entity<Muziek>(entity =>
            {
                entity.HasKey(e => e.MuziekID);
                entity.Property(e => e.MuziekID).ValueGeneratedOnAdd();
            });

            // Profiel configuratie
            modelBuilder.Entity<Profiel>(entity =>
            {
                entity.HasKey(e => e.ProfielID);
                entity.Property(e => e.ProfielID).ValueGeneratedOnAdd();
            });

            // Historiek configuratie
            modelBuilder.Entity<Historiek>(entity =>
            {
                entity.HasKey(e => e.HistoriekID);
                entity.Property(e => e.HistoriekID).ValueGeneratedOnAdd();
            });

            // MuziekAnalyse configuratie met relatie
            modelBuilder.Entity<MuziekAnalyse>(entity =>
            {
                entity.HasKey(e => e.AnalyseID);
                entity.Property(e => e.AnalyseID).ValueGeneratedOnAdd();
                
                entity.HasOne(e => e.Muziek)
                    .WithMany(m => m.Analyses)
                    .HasForeignKey(e => e.MuziekID)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Notificatie configuratie
            modelBuilder.Entity<Notificatie>(entity =>
            {
                entity.HasKey(e => e.NotificatieID);
                entity.Property(e => e.NotificatieID).ValueGeneratedOnAdd();
            });

            // Statistiek configuratie
            modelBuilder.Entity<Statistiek>(entity =>
            {
                entity.HasKey(e => e.StatistiekID);
                entity.Property(e => e.StatistiekID).ValueGeneratedOnAdd();
            });

            // StemmingType configuratie
            modelBuilder.Entity<StemmingType>(entity =>
            {
                entity.HasKey(e => e.TypeID);
                entity.Property(e => e.TypeID).ValueGeneratedOnAdd();
            });

            // Stemming configuratie met relaties
            modelBuilder.Entity<Stemming>(entity =>
            {
                entity.HasKey(e => e.StemmingID);
                entity.Property(e => e.StemmingID).ValueGeneratedOnAdd();
                
                entity.HasOne(e => e.StemmingType)
                    .WithMany(st => st.Stemmingen)
                    .HasForeignKey(e => e.TypeID)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // StemmingMuziek configuratie (koppeltabel)
            modelBuilder.Entity<StemmingMuziek>(entity =>
            {
                entity.HasKey(e => e.PK);
                entity.Property(e => e.PK).ValueGeneratedOnAdd();
                
                entity.HasOne(e => e.Stemming)
                    .WithMany(s => s.StemmingMuzieks)
                    .HasForeignKey(e => e.StemmingID)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasOne(e => e.Muziek)
                    .WithMany(m => m.StemmingMuzieks)
                    .HasForeignKey(e => e.MuziekID)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace HW04
{
    public class Context : DbContext
    {
        public DbSet<Publisher> Publishers => Set<Publisher>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Game> Games => Set<Game>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=hw04.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Publisher > Game (one-to-many)
            modelBuilder.Entity<Publisher>()
                .HasMany(p => p.Games)
                .WithOne(g => g.Publisher)
                .HasForeignKey(g => g.PublisherId);

            // Game <> Genre (many-to-many)
            modelBuilder.Entity<Game>()
                .HasMany(g => g.Genres)
                .WithMany(gn => gn.Games)
                .UsingEntity(j => j.ToTable("GameGenres"));
        }
    }
}
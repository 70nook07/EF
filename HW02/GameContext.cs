using Microsoft.EntityFrameworkCore;

namespace GameStore
{
    public class GameContext : DbContext
    {
        public DbSet<Game> Games => Set<Game>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=gamestore.db");
        }
    }
}
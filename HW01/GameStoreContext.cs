using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GameStore;

public class GameStoreContext : DbContext
{
    public DbSet<Developer> Developers => Set<Developer>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=gamestore.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .Property(g => g.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Developer>()
            .HasMany(d => d.Games)
            .WithOne(g => g.Developer)
            .HasForeignKey(g => g.DeveloperId);

        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<Game>()
            .HasMany(g => g.OrderItems)
            .WithOne(oi => oi.Game)
            .HasForeignKey(oi => oi.GameId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public static class DataSeeder
{
    public static void Seed(GameStoreContext context)
    {
        if (context.Developers.Any())
            return; // already seeded

        var developers = new List<Developer>
        {
            new() { Name = "CD Projekt Red", Country = "Poland" },
            new() { Name = "Naughty Dog", Country = "USA" },
            new() { Name = "FromSoftware", Country = "Japan" },
            new() { Name = "Rockstar Games", Country = "USA" },
            new() { Name = "Capcom", Country = "Japan" }
        };
        context.Developers.AddRange(developers);
        context.SaveChanges();

        var games = new List<Game>
        {
            new() { Title = "The Witcher 3: Wild Hunt", Price = 39.99m, ReleaseYear = 2015, DeveloperId = developers[0].Id },
            new() { Title = "Cyberpunk 2077", Price = 59.99m, ReleaseYear = 2020, DeveloperId = developers[0].Id },
            new() { Title = "The Last of Us Part II", Price = 49.99m, ReleaseYear = 2020, DeveloperId = developers[1].Id },
            new() { Title = "Uncharted 4", Price = 29.99m, ReleaseYear = 2016, DeveloperId = developers[1].Id },
            new() { Title = "Elden Ring", Price = 59.99m, ReleaseYear = 2022, DeveloperId = developers[2].Id },
            new() { Title = "Dark Souls III", Price = 39.99m, ReleaseYear = 2016, DeveloperId = developers[2].Id },
            new() { Title = "Grand Theft Auto V", Price = 29.99m, ReleaseYear = 2013, DeveloperId = developers[3].Id },
            new() { Title = "Red Dead Redemption 2", Price = 59.99m, ReleaseYear = 2018, DeveloperId = developers[3].Id },
            new() { Title = "Resident Evil 4 Remake", Price = 59.99m, ReleaseYear = 2023, DeveloperId = developers[4].Id },
            new() { Title = "Monster Hunter World", Price = 39.99m, ReleaseYear = 2018, DeveloperId = developers[4].Id }
        };
        context.Games.AddRange(games);
        context.SaveChanges();

        var customers = new List<Customer>
        {
            new() { FullName = "Ivan Petrenko", Email = "ivan.petrenko@example.com" },
            new() { FullName = "Olena Kovalenko", Email = "olena.kovalenko@example.com" },
            new() { FullName = "Mykola Shevchenko", Email = "mykola.shevchenko@example.com" },
            new() { FullName = "Anna Bondarenko", Email = "anna.bondarenko@example.com" },
            new() { FullName = "Taras Melnyk", Email = "taras.melnyk@example.com" },
            new() { FullName = "Kateryna Tkachenko", Email = "kateryna.tkachenko@example.com" },
            new() { FullName = "Dmytro Kravchenko", Email = "dmytro.kravchenko@example.com" },
            new() { FullName = "Yulia Moroz", Email = "yulia.moroz@example.com" }
        };
        context.Customers.AddRange(customers);
        context.SaveChanges();

        var orders = new List<Order>
        {
            new() { CustomerId = customers[0].Id, OrderDate = new DateTime(2024, 1, 5) },
            new() { CustomerId = customers[1].Id, OrderDate = new DateTime(2024, 1, 10) },
            new() { CustomerId = customers[0].Id, OrderDate = new DateTime(2024, 1, 15) },
            new() { CustomerId = customers[2].Id, OrderDate = new DateTime(2024, 2, 1) },
            new() { CustomerId = customers[3].Id, OrderDate = new DateTime(2024, 2, 5) },
            new() { CustomerId = customers[3].Id, OrderDate = new DateTime(2024, 2, 20) },
            new() { CustomerId = customers[4].Id, OrderDate = new DateTime(2024, 3, 1) },
            new() { CustomerId = customers[5].Id, OrderDate = new DateTime(2024, 3, 10) },
            new() { CustomerId = customers[6].Id, OrderDate = new DateTime(2024, 3, 15) },
            new() { CustomerId = customers[7].Id, OrderDate = new DateTime(2024, 3, 20) }
        };
        context.Orders.AddRange(orders);
        context.SaveChanges();

        var orderItems = new List<OrderItem>
        {
            new() { OrderId = orders[0].Id, GameId = games[0].Id, Quantity = 1 },
            new() { OrderId = orders[0].Id, GameId = games[1].Id, Quantity = 1 },
            new() { OrderId = orders[1].Id, GameId = games[2].Id, Quantity = 2 },
            new() { OrderId = orders[1].Id, GameId = games[3].Id, Quantity = 1 },
            new() { OrderId = orders[2].Id, GameId = games[4].Id, Quantity = 1 },
            new() { OrderId = orders[2].Id, GameId = games[5].Id, Quantity = 1 },
            new() { OrderId = orders[3].Id, GameId = games[6].Id, Quantity = 3 },
            new() { OrderId = orders[3].Id, GameId = games[7].Id, Quantity = 1 },
            new() { OrderId = orders[4].Id, GameId = games[8].Id, Quantity = 1 },
            new() { OrderId = orders[4].Id, GameId = games[9].Id, Quantity = 1 },
            new() { OrderId = orders[5].Id, GameId = games[0].Id, Quantity = 1 },
            new() { OrderId = orders[5].Id, GameId = games[2].Id, Quantity = 1 },
            new() { OrderId = orders[6].Id, GameId = games[1].Id, Quantity = 2 },
            new() { OrderId = orders[6].Id, GameId = games[4].Id, Quantity = 1 },
            new() { OrderId = orders[7].Id, GameId = games[5].Id, Quantity = 1 },
            new() { OrderId = orders[7].Id, GameId = games[6].Id, Quantity = 1 },
            new() { OrderId = orders[8].Id, GameId = games[7].Id, Quantity = 2 },
            new() { OrderId = orders[8].Id, GameId = games[8].Id, Quantity = 1 },
            new() { OrderId = orders[9].Id, GameId = games[9].Id, Quantity = 1 },
            new() { OrderId = orders[9].Id, GameId = games[3].Id, Quantity = 2 }
        };
        context.OrderItems.AddRange(orderItems);
        context.SaveChanges();
    }
}
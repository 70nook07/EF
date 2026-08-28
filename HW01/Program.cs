using System;
using System.Linq;
using GameStore;
using Microsoft.EntityFrameworkCore;

using var context = new GameStoreContext();
context.Database.EnsureCreated();
DataSeeder.Seed(context);

Console.WriteLine("=== 1. Games with developers ===");
var gamesWithDevelopers = context.Games
    .Include(g => g.Developer)
    .OrderBy(g => g.Title)
    .ToList();

foreach (var game in gamesWithDevelopers)
    Console.WriteLine($"{game.Title} ({game.ReleaseYear}) - {game.Developer.Name}");

Console.WriteLine("\n=== 2. Orders with customers and games ===");
var ordersWithDetails = context.Orders
    .Include(o => o.Customer)
    .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Game)
    .OrderBy(o => o.Id)
    .ToList();

foreach (var order in ordersWithDetails)
{
    Console.WriteLine($"Order #{order.Id} | {order.Customer.FullName} | {order.OrderDate:yyyy-MM-dd}");
    foreach (var item in order.OrderItems)
        Console.WriteLine($"   - {item.Game.Title} x{item.Quantity}");
}

Console.WriteLine("\n=== 3. Total amount per order ===");
var orderTotals = context.Orders
    .Select(o => new
    {
        o.Id,
        Customer = o.Customer.FullName,
        Total = o.OrderItems.Sum(oi => oi.Quantity * oi.Game.Price)
    })
    .OrderBy(o => o.Id)
    .ToList();

foreach (var order in orderTotals)
    Console.WriteLine($"Order #{order.Id} ({order.Customer}): {order.Total:C}");

Console.WriteLine("\n=== 4. Top 3 most expensive games ===");
var topGames = context.Games
    .OrderByDescending(g => g.Price)
    .Take(3)
    .ToList();

foreach (var game in topGames)
    Console.WriteLine($"{game.Title}: {game.Price:C}");

Console.WriteLine("\n=== 5. Customers with more than 1 order ===");
var repeatCustomers = context.Customers
    .Where(c => c.Orders.Count > 1)
    .Select(c => new { c.FullName, OrderCount = c.Orders.Count })
    .ToList();

foreach (var customer in repeatCustomers)
    Console.WriteLine($"{customer.FullName}: {customer.OrderCount} orders");

Console.WriteLine("\n=== 6. Total store revenue ===");
var totalRevenue = context.OrderItems
    .Sum(oi => oi.Quantity * oi.Game.Price);

Console.WriteLine($"{totalRevenue:C}");
using GameStore;
using Microsoft.EntityFrameworkCore;

using var db = new GameContext();

// Task 3: create the database and apply migrations
db.Database.Migrate();

if (!db.Games.Any())
{
    db.Games.AddRange(
        new Game { Title = "The Witcher 3", Studio = "CD Projekt Red", Genre = "RPG",
            ReleaseDate = new DateTime(2015, 5, 19), Mode = GameMode.SinglePlayer, CopiesSold = 50_000_000 },
        new Game { Title = "Counter-Strike 2", Studio = "Valve", Genre = "Shooter",
            ReleaseDate = new DateTime(2023, 9, 27), Mode = GameMode.Multiplayer, CopiesSold = 30_000_000 },
        new Game { Title = "Stardew Valley", Studio = "ConcernedApe", Genre = "Simulation",
            ReleaseDate = new DateTime(2016, 2, 26), Mode = GameMode.SinglePlayer, CopiesSold = 20_000_000 }
    );
    db.SaveChanges();
}

Console.WriteLine("Games in database:");
foreach (var game in db.Games)
{
    Console.WriteLine(
        $"{game.Title} | {game.Studio} | {game.Genre} | {game.ReleaseDate:yyyy-MM-dd} | {game.Mode} | {game.CopiesSold:N0} copies");
}
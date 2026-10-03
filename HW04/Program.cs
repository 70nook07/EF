using HW04;
using Microsoft.EntityFrameworkCore;

using var db = new Context();
db.Database.EnsureCreated();

if (!db.Publishers.Any())
{
    PublisherService.Create(db, "CD Projekt Red", "Poland", 1994, "https://cdprojektred.com");
    PublisherService.Create(db, "Electronic Arts", "USA", 1982, "https://ea.com");
}

if (!db.Genres.Any())
{
    GenreService.Create(db, "RPG");
    GenreService.Create(db, "Action");
    GenreService.Create(db, "Sports");
}

if (!db.Games.Any())
{
    var cdpr = db.Publishers.First(p => p.Name == "CD Projekt Red");
    var ea = db.Publishers.First(p => p.Name == "Electronic Arts");

    var witcher = GameService.Create(db, "The Witcher 3", 39.99m, 2015, "Open world RPG", cdpr.Id);
    var cyberpunk = GameService.Create(db, "Cyberpunk 2077", 59.99m, 2020, "Sci-fi RPG", cdpr.Id);
    var fifa = GameService.Create(db, "FIFA 24", 69.99m, 2023, "Football simulator", ea.Id);

    var rpg = db.Genres.First(g => g.Name == "RPG");
    var action = db.Genres.First(g => g.Name == "Action");
    var sports = db.Genres.First(g => g.Name == "Sports");

    GameService.AddGenre(db, witcher.Id, rpg.Id);
    GameService.AddGenre(db, witcher.Id, action.Id);
    GameService.AddGenre(db, cyberpunk.Id, rpg.Id);
    GameService.AddGenre(db, fifa.Id, sports.Id);
}

Console.WriteLine("~~~ All games with genres (eager loading) ~~~");
foreach (var game in GameService.GetAllWithGenres(db))
    Console.WriteLine($"{game.Title}: {string.Join(", ", game.Genres.Select(gn => gn.Name))}");

Console.WriteLine("\n~~~ Game's publisher (explicit loading) ~~~");
var witcherGame = db.Games.First(g => g.Title == "The Witcher 3");
var loadedGame = GameService.GetByIdWithPublisherExplicit(db, witcherGame.Id);
Console.WriteLine($"{loadedGame!.Title} is published by {loadedGame.Publisher.Name}");

Console.WriteLine("\n~~~ Games of genre 'RPG' ~~~");
var rpgGenre = db.Genres.First(g => g.Name == "RPG");
GenreService.PrintGames(db, rpgGenre.Id);

Console.WriteLine("\n~~~ Genres of 'The Witcher 3' ~~~");
GameService.PrintGenres(db, witcherGame.Id);

Console.WriteLine("\n~~~ Games by publisher 'CD Projekt Red' ~~~");
var cdprPublisher = db.Publishers.First(p => p.Name == "CD Projekt Red");
PublisherService.PrintGames(db, cdprPublisher.Id);

Console.WriteLine("\n~~~ Update demo ~~~");
GameService.Update(db, witcherGame.Id, "The Witcher 3: Wild Hunt", 29.99m, 2015, "GOTY edition", cdprPublisher.Id);
Console.WriteLine("Game updated.");

Console.WriteLine("\n~~~ Delete demo ~~~");
var fifaGame = db.Games.First(g => g.Title == "FIFA 24");
GameService.Delete(db, fifaGame.Id);
Console.WriteLine("Game deleted.");


public static class GameService
{
    public static Game Create(Context db, string title, decimal price, int releaseYear, string description, int publisherId)
    {
        var game = new Game { Title = title, Price = price, ReleaseYear = releaseYear, Description = description, PublisherId = publisherId };
        db.Games.Add(game);
        db.SaveChanges();
        return game;
    }

    public static List<Game> GetAll(Context db) => db.Games.ToList();

    public static Game? GetById(Context db, int id) => db.Games.Find(id);

    // Eager loading
    public static List<Game> GetAllWithGenres(Context db) =>
        db.Games.Include(g => g.Genres).ToList();

    // Explicit loading
    public static Game? GetByIdWithPublisherExplicit(Context db, int id)
    {
        var game = db.Games.FirstOrDefault(g => g.Id == id);
        if (game != null)
            db.Entry(game).Reference(g => g.Publisher).Load();
        return game;
    }

    public static bool Update(Context db, int id, string title, decimal price, int releaseYear, string description, int publisherId)
    {
        var game = db.Games.Find(id);
        if (game == null) return false;

        game.Title = title;
        game.Price = price;
        game.ReleaseYear = releaseYear;
        game.Description = description;
        game.PublisherId = publisherId;
        db.SaveChanges();
        return true;
    }

    public static bool Delete(Context db, int id)
    {
        var game = db.Games.Find(id);
        if (game == null) return false;

        db.Games.Remove(game);
        db.SaveChanges();
        return true;
    }

    public static void AddGenre(Context db, int gameId, int genreId)
    {
        var game = db.Games.Include(g => g.Genres).FirstOrDefault(g => g.Id == gameId);
        var genre = db.Genres.Find(genreId);
        if (game == null || genre == null) return;

        if (!game.Genres.Any(g => g.Id == genreId))
            game.Genres.Add(genre);
        db.SaveChanges();
    }

    // Genres of the game (eager loading)
    public static void PrintGenres(Context db, int gameId)
    {
        var game = db.Games.Include(g => g.Genres).FirstOrDefault(g => g.Id == gameId);
        if (game == null) { Console.WriteLine("Game not found."); return; }

        foreach (var genre in game.Genres)
            Console.WriteLine($" - {genre.Name}");
    }
}

public static class GenreService
{
    public static Genre Create(Context db, string name)
    {
        var genre = new Genre { Name = name };
        db.Genres.Add(genre);
        db.SaveChanges();
        return genre;
    }

    public static List<Genre> GetAll(Context db) => db.Genres.ToList();

    public static Genre? GetById(Context db, int id) => db.Genres.Find(id);

    public static bool Update(Context db, int id, string name)
    {
        var genre = db.Genres.Find(id);
        if (genre == null) return false;

        genre.Name = name;
        db.SaveChanges();
        return true;
    }

    public static bool Delete(Context db, int id)
    {
        var genre = db.Genres.Find(id);
        if (genre == null) return false;

        db.Genres.Remove(genre);
        db.SaveChanges();
        return true;
    }

    // Games of the genre (eager loading)
    public static void PrintGames(Context db, int genreId)
    {
        var genre = db.Genres.Include(g => g.Games).FirstOrDefault(g => g.Id == genreId);
        if (genre == null) { Console.WriteLine("Genre not found."); return; }

        foreach (var game in genre.Games)
            Console.WriteLine($" - {game.Title}");
    }
}

public static class PublisherService
{
    public static Publisher Create(Context db, string name, string country, int foundedYear, string website)
    {
        var publisher = new Publisher { Name = name, Country = country, FoundedYear = foundedYear, Website = website };
        db.Publishers.Add(publisher);
        db.SaveChanges();
        return publisher;
    }

    public static List<Publisher> GetAll(Context db) => db.Publishers.ToList();

    public static Publisher? GetById(Context db, int id) => db.Publishers.Find(id);

    public static bool Update(Context db, int id, string name, string country, int foundedYear, string website)
    {
        var publisher = db.Publishers.Find(id);
        if (publisher == null) return false;

        publisher.Name = name;
        publisher.Country = country;
        publisher.FoundedYear = foundedYear;
        publisher.Website = website;
        db.SaveChanges();
        return true;
    }

    public static bool Delete(Context db, int id)
    {
        var publisher = db.Publishers.Find(id);
        if (publisher == null) return false;

        db.Publishers.Remove(publisher);
        db.SaveChanges();
        return true;
    }

    // Games of the publisher (explicit loading)
    public static void PrintGames(Context db, int publisherId)
    {
        var publisher = db.Publishers.FirstOrDefault(p => p.Id == publisherId);
        if (publisher == null) { Console.WriteLine("Publisher not found."); return; }

        db.Entry(publisher).Collection(p => p.Games).Load();

        foreach (var game in publisher.Games)
            Console.WriteLine($" - {game.Title}");
    }
}

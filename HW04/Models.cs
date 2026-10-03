namespace HW04
{
    public class Publisher
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int FoundedYear { get; set; }
        public string Website { get; set; } = string.Empty;

        public List<Game> Games { get; set; } = new();
    }

    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<Game> Games { get; set; } = new();
    }

    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int ReleaseYear { get; set; }
        public string Description { get; set; } = string.Empty;

        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; } = null!;

        public List<Genre> Genres { get; set; } = new();
    }
}
using System;

namespace GameStore
{
    public enum GameMode
    {
        SinglePlayer,
        Multiplayer
    }

    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Studio { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }

        // Task 4: added later, requires a new migration
        public GameMode Mode { get; set; }
        public long CopiesSold { get; set; }
    }
}
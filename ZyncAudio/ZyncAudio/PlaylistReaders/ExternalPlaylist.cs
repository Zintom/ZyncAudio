using System.Collections.Generic;

namespace ZyncAudio.PlaylistReaders
{
    /// <summary>
    /// Represents information regarding a music playlist.
    /// </summary>
    public class ExternalPlaylist
    {
        public string? Title { get; set; }

        public string? Generator { get; set; }

        public int? AverageRating { get; set; }

        public int? TotalDuration { get; set; }

        public int? ItemCount { get; set; }

        public List<string> MediaItems { get; } = new();
    }
}

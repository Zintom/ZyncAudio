using System.IO;

namespace ZyncAudio.PlaylistReaders
{
    /// <summary>
    /// A playlist reader can intepret and convert a file into an <see cref="ExternalPlaylist"/>. This is an abstract class.
    /// </summary>
    public abstract class PlaylistReader
    {
        /// <summary>
        /// Reads the file at the given `<paramref name="filePath"/>` and interprets it as an <see cref="ExternalPlaylist"/>.
        /// </summary>
        public ExternalPlaylist Read(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Read(stream, filePath);
            }
        }

        /// <summary>
        /// Reads the `<paramref name="stream"/>` and interprets it as an <see cref="ExternalPlaylist"/>.
        /// </summary>
        /// <param name="playlistFilePath">The location of the playlist file, enables the reader to interprety Relative Paths correctly, such as `..\example.mp3`</param>
        public abstract ExternalPlaylist Read(Stream stream, string? playlistFilePath);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using ZyncAudio.Extensions;
using System.Security.Cryptography;

namespace ZyncAudio.Host
{
    public class Playlist
    {

        public enum PlayingMode
        {
            /// <summary>
            /// The default mode, <c>MoveNext()</c> or <c>MovePrevious()</c> loop the track-list in a circular fashion.
            /// </summary>
            LoopAll,
            /// <summary>
            /// Any calls to <c>MoveNext()</c> or <c>MovePrevious()</c> are effectively NOPs, <see cref="Current"/> won't change in this mode.
            /// </summary>
            LoopSingle,
        }

        /// <summary>
        /// Determines the current rule-set(mode) that will be used for each call to <c>MoveNext()</c> and <c>MovePrevious()</c>.
        /// </summary>
        public PlayingMode Mode { get; set; } = PlayingMode.LoopAll;

        /// <summary>
        /// The index of the current track in the playlist.
        /// </summary>
        public int Position { get; set; } = 0;

        private List<string> _trackFilePaths = new();

        /// <summary>
        /// Returns an enumerable collection of all the track file paths in the playlist.
        /// </summary>
        public IEnumerable<string> Tracks => _trackFilePaths;

        /// <summary>
        /// The current item selected by the enumeration.
        /// </summary>
        public string? Current
        {
            get
            {
                if (Position < 0 || Position >= _trackFilePaths.Count || _trackFilePaths.Count == 0)
                    return null;

                return _trackFilePaths[Position];
            }
        }

        public int TrackListSize { get => _trackFilePaths.Count; }

        /// <summary>
        /// Add's the given <paramref name="trackFilePath"/> to the playlist.
        /// </summary>
        public void Add(string trackFilePath)
        {
            _trackFilePaths.Add(trackFilePath);
        }

        public void Clear()
        {
            Position = 0;
            _trackFilePaths.Clear();
        }

        /// <summary>
        /// Shuffles the track list.
        /// </summary>
        public void Shuffle()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                string[] newPositions = new string[_trackFilePaths.Count];
                List<int> availablePositionPot = new(_trackFilePaths.Count);

                // Fill available position with ordered indexes.
                for (int i = 0; i < _trackFilePaths.Count; i++) { availablePositionPot.Add(i); }

                for (int i = 0; i < _trackFilePaths.Count; i++)
                {
                    // take an available position from the pot
                    int newPositionIndex = GetRandomNumber(availablePositionPot.Count, rng);
                    int newPos = availablePositionPot[newPositionIndex];
                    availablePositionPot.RemoveAt(newPositionIndex);

                    newPositions[newPos] = _trackFilePaths[i];
                }

                _trackFilePaths = newPositions.ToList();
            }

            static int GetRandomNumber(int maxValue, RandomNumberGenerator rng)
            {
                Span<byte> output = stackalloc byte[4];
                rng.GetBytes(output);
                return BitConverter.ToInt32(output).Mod(maxValue);
            }
        }

        /// <summary>
        /// Almost identical to <c>MoveNext()</c> however moves backwards.
        /// </summary>
        /// <param name="positions">If in <see cref="PlayingMode.LoopAll"/>, how many track positions should we jump backward.</param>
        /// <remarks>Behaviour depends on the current <see cref="PlayingMode"/>. <b>Can block if Mode equals</b> <see cref="PlayingMode.Pause"/></remarks>
        /// <returns>Whether we have come to the end of the enumeration.</returns>
        public bool MovePrevious(int positions = 1)
        {
            if (TrackListSize == 0) return false;

            switch (Mode)
            {
                case PlayingMode.LoopAll:
                    // We use Modulo to ensure the position lies within the bounds of the track list.
                    Position = (Position - positions).Mod(TrackListSize);
                    break;
                case PlayingMode.LoopSingle:
                    break;
            }

            return true;
        }

        /// <summary>
        /// Moves to the next item in the track-list.
        /// </summary>
        /// <param name="positions">If in <see cref="PlayingMode.LoopAll"/>, how many track positions should we jump forward.</param>
        /// <remarks>Behaviour depends on the current <see cref="PlayingMode"/>. <b>Can block if Mode equals</b> <see cref="PlayingMode.Pause"/></remarks>
        /// <returns>Whether we have come to the end of the enumeration.</returns>
        public bool MoveNext(int positions = 1)
        {
            if (TrackListSize == 0) return false;

            switch (Mode)
            {
                case PlayingMode.LoopAll:
                    // We use Modulo to ensure the position lies within the bounds of the track list.
                    Position = (Position + positions).Mod(TrackListSize);
                    break;
                case PlayingMode.LoopSingle:
                    break;
            }

            return true;
        }

    }
}

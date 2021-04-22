using System;
using System.Collections.Generic;
using ZyncAudio.Extensions;

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

        private readonly List<string> _trackFilePaths = new();

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
        /// Almost identical to <c>MoveNext()</c> however moves backwards.
        /// </summary>
        /// <param name="positions">If in <see cref="PlayingMode.LoopAll"/>, how many track positions should we jump backward.</param>
        /// <remarks>Behaviour depends on the current <see cref="PlayingMode"/>. <b>Can block if Mode equals</b> <see cref="PlayingMode.Pause"/></remarks>
        /// <returns>Whether we have come to the end of the enumeration.</returns>
        public bool MovePrevious(int positions = 1)
        {
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

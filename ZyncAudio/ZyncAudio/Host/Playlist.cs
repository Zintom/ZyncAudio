using System.Collections.Generic;
using System.Threading;

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
            /// <summary>
            /// Effectively pauses the playlist from proceeding to the next song.
            /// <para/>
            /// <c>MoveNext()</c> or <c>MovePrevious()</c> will halt until the state changes out of <see cref="Pause"/>.
            /// </summary>
            Pause,
            /// <summary>
            /// Any calls to <c>MoveNext()</c> or <c>MovePrevious()</c> will return false indicating the end of enumeration.
            /// </summary>
            Stop
        }

        private PlayingMode _mode = PlayingMode.Stop;
        /// <summary>
        /// Determines the current rule-set(mode) that will be used for each call to <c>MoveNext()</c> and <c>MovePrevious()</c>.
        /// </summary>
        public PlayingMode Mode
        {
            get => _mode;
            set
            {
                if(value != PlayingMode.Pause)
                {
                    _waitForUnpause.Set();
                }

                _mode = value;
            }
        }

        private int _position = 0;
        /// <summary>
        /// The index of the current track in the playlist.
        /// </summary>
        public int Position
        {
            get => _position;
            set
            {
                if (value < 0 || value >= _trackFilePaths.Count) { return; }

                _position = value;
            }
        }

        private readonly ManualResetEvent _waitForUnpause = new(false);

        private readonly List<string> _trackFilePaths = new();

        /// <summary>
        /// The current item selected by the enumeration.
        /// </summary>
        public string? Current
        {
            get
            {
                if (_position < 0 || _position >= _trackFilePaths.Count)
                    return null;

                return _trackFilePaths[_position];
            }
        }

        /// <summary>
        /// Add's the given <paramref name="trackFilePath"/> to the playlist.
        /// </summary>
        public void Add(string trackFilePath)
        {
            _trackFilePaths.Add(trackFilePath);
        }

        /// <summary>
        /// Almost identical to <c>MoveNext()</c> however moves backwards.
        /// </summary>
        /// <remarks>Behaviour depends on the current <see cref="PlayingMode"/>. <b>Can block if Mode equals</b> <see cref="PlayingMode.Pause"/></remarks>
        /// <returns>Whether we have come to the end of the enumeration.</returns>
        public bool MovePrevious()
        {
            switch (Mode)
            {
                case PlayingMode.LoopAll:
                    if (_position - 1 < 0)
                    {
                        _position = _trackFilePaths.Count - 1;
                    }
                    else
                    {
                        _position--;
                    }
                    break;
                case PlayingMode.LoopSingle:
                    break;
                case PlayingMode.Pause:
                    _waitForUnpause.WaitOne();
                    break;
                case PlayingMode.Stop:
                    _position = -1;
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Moves to the next item in the track-list.
        /// </summary>
        /// <remarks>Behaviour depends on the current <see cref="PlayingMode"/>. <b>Can block if Mode equals</b> <see cref="PlayingMode.Pause"/></remarks>
        /// <returns>Whether we have come to the end of the enumeration.</returns>
        public bool MoveNext()
        {
            switch (Mode)
            {
                case PlayingMode.LoopAll:
                    if (_position + 1 >= _trackFilePaths.Count)
                    {
                        _position = 0;
                    }
                    else
                    {
                        _position++;
                    }
                    break;
                case PlayingMode.LoopSingle:
                    break;
                case PlayingMode.Pause:
                    _waitForUnpause.WaitOne();
                    break;
                case PlayingMode.Stop:
                    _position = -1;
                    return false;
            }

            return true;
        }

    }
}

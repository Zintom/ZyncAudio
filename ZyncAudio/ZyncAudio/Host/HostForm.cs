using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using ZyncAudio.Host;
using System.Windows.Forms;
using Zintom.StorageFacility;
using ZyncAudio.PlaylistReaders;

namespace ZyncAudio
{
    public partial class HostForm : Form
    {

        private enum GUIState
        {
            ServerOpenToEntry,
            ServerEntryClosed,
            AudioRerouterOpened
        }

        private Server _server;

        private AudioServer _audioServer;

        private ILogger _logger;

        private readonly Playlist _playlist = new();

        private static readonly TimeSpan _preBufferSize = TimeSpan.FromSeconds(2);

        public HostForm()
        {
            InitializeComponent();

            // Init list view
            ClientListView.View = View.Details;
            ClientListView.FullRowSelect = true;
            ClientListView.MultiSelect = false;
            ClientListView.Columns.Add("address", "Address", 124);
            ClientListView.Columns.Add("ping", "Ping", 50);

            _logger = new ConsoleLogger();
            _server = new Server(_logger);
            _audioServer = new AudioServer(_server);

            _server.Open(IPAddress.Any, 60759);
            _server.ClientConnected = ClientConnected;
            _server.ClientDisconnected = ClientDisconnected;

            _audioServer.PlaybackStarted += PlaybackStarted;
            _audioServer.PlaybackStoppedNaturally += PlaybackStoppedNaturally;

            var settings = Storage.GetStorage(Program.SettingsFile);
            _volumeControlBar.Value = settings.Integers.GetValue("volumeSliderValue", 100);
            VolumeControlBar_Scroll(null!, null!);
            _searchSubFoldersBtn.Checked = settings.Booleans.GetValue("searchSubFoldersOnLoadFolder", false);

            ChangeGUIState(GUIState.ServerOpenToEntry);
        }

        private void ClientConnected(Socket client)
        {
            Invoke(new Action(() =>
            {
                _closeEntryBtn.Enabled = true;

                ClientListView.Items.Add(new ListViewItem(new string[] { client.RemoteEndPoint?.ToString() ?? "null", "0ms" }));

                // Ensure client is set to the correct audio volume level.
                _audioServer.ChangeVolume(_volumeControlBar.Value / 100f);
            }));
        }

        private void ClientDisconnected(Socket client)
        {
            Invoke(new Action(() =>
            {
                string remoteAddress = client.RemoteEndPoint?.ToString() ?? "null";
                foreach (ListViewItem item in ClientListView.Items)
                {
                    if (item.Text == remoteAddress)
                    {
                        ClientListView.Items.Remove(item);
                        break;
                    }
                }
            }));
        }

        private void HostForm_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Changes the GUI based upon the given <paramref name="state"/>
        /// </summary>
        private void ChangeGUIState(GUIState state)
        {
            switch (state)
            {
                case GUIState.ServerOpenToEntry:
                    _closeEntryBtn.Enabled = true;
                    _playBtn.Enabled = false;
                    _stopBtn.Enabled = false;
                    _previousBtn.Enabled = false;
                    _nextBtn.Enabled = false;
                    _playListView.Enabled = false;
                    _loadFolderBtn.Enabled = false;
                    _addSingleTrackBtn.Enabled = false;
                    _unloadPlaylistBtn.Enabled = false;
                    _shuffleBtn.Enabled = false;
                    _searchSubFoldersBtn.Enabled = false;
                    _rerouteAudioBtn.Enabled = false;
                    _audioLevelsImg.Enabled = false;
                    _volumeControlBar.Enabled = false;
                    break;
                case GUIState.ServerEntryClosed:
                    _closeEntryBtn.Enabled = false;
                    _playBtn.Enabled = true;
                    _stopBtn.Enabled = true;
                    _previousBtn.Enabled = true;
                    _nextBtn.Enabled = true;
                    _playListView.Enabled = true;
                    _loadFolderBtn.Enabled = true;
                    _addSingleTrackBtn.Enabled = true;
                    _unloadPlaylistBtn.Enabled = true;
                    _shuffleBtn.Enabled = true;
                    _searchSubFoldersBtn.Enabled = true;
                    _rerouteAudioBtn.Enabled = true;
                    _audioLevelsImg.Enabled = true;
                    _volumeControlBar.Enabled = true;
                    _trackScrubBar.Value = 0;
                    SetPausedState(true, 0L);
                    DisableScrubBar();
                    break;
                case GUIState.AudioRerouterOpened:
                    _closeEntryBtn.Enabled = false;
                    _playBtn.Enabled = false;
                    _stopBtn.Enabled = false;
                    _previousBtn.Enabled = false;
                    _nextBtn.Enabled = false;
                    _playListView.Enabled = false;
                    _loadFolderBtn.Enabled = false;
                    _addSingleTrackBtn.Enabled = false;
                    _unloadPlaylistBtn.Enabled = false;
                    _shuffleBtn.Enabled = false;
                    _searchSubFoldersBtn.Enabled = false;
                    _rerouteAudioBtn.Enabled = false;
                    _audioLevelsImg.Enabled = true;
                    _volumeControlBar.Enabled = true;
                    _trackScrubBar.Value = 0;
                    DisableScrubBar();
                    break;
            }
        }

        private bool _paused = true;
        private long _pausedOnMilliseconds = 0L;
        private readonly object _mediaActionLockObject = new();
        private void PlayBtn_Click(object sender, EventArgs e)
        {
            lock (_mediaActionLockObject)
            {
                if (_paused)
                {
                    _audioServer.PlayAsync(_playlist.Current, TimeSpan.FromMilliseconds(_pausedOnMilliseconds), _preBufferSize);

                    SetPausedState(false, 0L);
                }
                else
                {
                    SetPausedState(true, (long)_audioServer.CurrentTrackElapsedTime.TotalMilliseconds);
                    _audioServer.Stop(false);
                }

                RefreshNowPlaying();
            }
        }

        private void SetPausedState(bool paused, long pausedMillisecondPosition = 0L)
        {
            if (paused)
            {
                _paused = true;
                _pausedOnMilliseconds = pausedMillisecondPosition;
                _playBtn.BackgroundImage = Properties.Resources.media_play_8x;
                _toolTipProvider.SetToolTip(_playBtn, "Plays the current track.");
            }
            else
            {
                _paused = false;
                _pausedOnMilliseconds = 0L;
                _playBtn.BackgroundImage = Properties.Resources.media_pause_8x;
                _toolTipProvider.SetToolTip(_playBtn, "Pauses the current track.");
            }
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            lock (_mediaActionLockObject)
            {
                _audioServer.Stop(true);

                RefreshNowPlaying();
                SetPausedState(true);
                DisableScrubBar();

                _audioServer.ChangeNowPlayingInfo(Program.NoAudioPlaying);
            }
        }

        private void DisableScrubBar(bool resetPosition = true)
        {
            _trackScrubBar.Enabled = false;
            if (resetPosition) { _trackScrubBar.Value = 0; }
            _trackElapsedTimeLbl.Enabled = false;
            _trackElapsedTimeLbl.Text = "00:00";
            _trackElapsedTimeTicker.Stop();
        }

        private void CloseEntryBtn_Click(object sender, EventArgs e)
        {
            _server.StopAccepting();
            ChangeGUIState(GUIState.ServerEntryClosed);
            //_closeEntryBtn.Enabled = false;

            //_playBtn.Enabled = true;
            //_stopBtn.Enabled = true;
            //_previousBtn.Enabled = true;
            //_nextBtn.Enabled = true;
            //_playListView.Enabled = true;
            //_loadFolderBtn.Enabled = true;
            //_unloadPlaylistBtn.Enabled = true;
            //_shuffleBtn.Enabled = true;
            //_searchSubFoldersBtn.Enabled = true;
            //_rerouteAudioBtn.Enabled = true;
        }

        private void PingChecker_Tick(object sender, EventArgs e)
        {
            foreach (var pair in _audioServer.Pinger.PingStatistics)
            {
                string ping = pair.Value + "ms";
                string address = pair.Key.RemoteEndPoint?.ToString() ?? "null";

                foreach (ListViewItem item in ClientListView.Items)
                {
                    if (item.Text == address)
                    {
                        item.SubItems[1].Text = ping;
                    }
                }
            }
        }

        private void HostForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _audioServer.Stop(true);
            _server.Close();

            var settingsEditor = Storage.GetStorage(Program.SettingsFile).Edit();
            settingsEditor.PutValue("volumeSliderValue", _volumeControlBar.Value)
                          .PutValue("searchSubFoldersOnLoadFolder", _searchSubFoldersBtn.Checked).Commit();
        }

        private void LoadFolderBtn_Click(object sender, EventArgs e)
        {
            using (var folderBrowser = new FolderBrowserDialog())
            {
                var result = folderBrowser.ShowDialog();
                if (result != DialogResult.OK) { return; }

                string folder = folderBrowser.SelectedPath;

                foreach (var file in Directory.GetFiles(folder,
                                                        "",
                                                        _searchSubFoldersBtn.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                {
                    if (file.EndsWith(".wav")
                        || file.EndsWith(".mp3")
                        || file.EndsWith(".flac"))
                    {
                        AddTrackToPlaylist(file);
                    }
                }
            }
        }

        private void AddSingleTrackBtn_Clicked(object sender, EventArgs e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                var result = fileDialog.ShowDialog();
                if (result != DialogResult.OK) { return; }

                string file = fileDialog.FileName;

                if (file.EndsWith(".wav")
                    || file.EndsWith(".mp3")
                    || file.EndsWith(".flac"))
                {
                    AddTrackToPlaylist(file);
                }
                else if (file.EndsWith(".wpl") || file.EndsWith(".m3u"))
                {
                    LoadPlaylist(file);
                }
            }
        }

        /// <summary>
        /// Adds the given <paramref name="trackFilePath"/> to the playlist.
        /// </summary>
        private void AddTrackToPlaylist(string trackFilePath)
        {
            _playlist.Add(trackFilePath);
            _playListView.Items.Add(new FileInfo(trackFilePath).Name);
        }

        private void LoadPlaylist(string file)
        {
            PlaylistReader playlistReader = null!;

            if (file.EndsWith(".wpl"))
            {
                playlistReader = new WPLReader();
            }
            else if (file.EndsWith(".m3u"))
            {
                playlistReader = null!;
            }

            // Only ask to clear the current playlist if it
            // actually has any items.
            if (_playlist.TrackListSize > 0)
            {
                var dialogResult = MessageBox.Show(caption: "How shall we load the playlist?", text: "Erase the current playlist?", buttons: MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    _audioServer.Stop(true);
                    _playlist.Clear();
                    _playListView.Items.Clear();
                }
            }

            foreach (string track in playlistReader.Read(file).MediaItems)
            {
                AddTrackToPlaylist(track);
            }
        }

        private void UnloadItems_Click(object sender, EventArgs e)
        {
            _playlist.Clear();
            _playListView.Items.Clear();
        }

        private void PlaybackStarted(AudioServer.TrackInformation trackInfo)
        {
            Invoke(new Action(() =>
            {
                if (trackInfo.Type == AudioServer.TrackInformation.TrackType.LiveStream)
                {
                    _trackScrubBar.Enabled = false;
                    _trackScrubBar.Value = 0;
                    return;
                }

                _trackScrubBar.Enabled = true;
                _trackScrubBar.Maximum = (int)trackInfo.Length;
                _trackElapsedTimeTicker.Start();
            }));
        }

        private void PlaybackStoppedNaturally()
        {
            Invoke(new Action(() => DisableScrubBar(true)));
            _playlist.MoveNext();
            _audioServer.PlayAsync(_playlist.Current, TimeSpan.Zero, _preBufferSize);

            RefreshNowPlaying();
        }

        private void NextBtn_Click(object sender, EventArgs e)
        {
            lock (_mediaActionLockObject)
            {
                _playlist.MoveNext();
                _audioServer.Stop(true);
                _audioServer.PlayAsync(_playlist.Current, TimeSpan.Zero, _preBufferSize);

                if (_playlist.Current == null)
                {
                    RefreshNowPlaying();
                }

                SetPausedState(false);
                DisableScrubBar();
                RefreshNowPlaying();
            }
        }

        private void PreviousBtn_Click(object sender, EventArgs e)
        {
            lock (_mediaActionLockObject)
            {
                _playlist.MovePrevious();
                _audioServer.Stop(true);
                _audioServer.PlayAsync(_playlist.Current, TimeSpan.Zero, _preBufferSize);

                if (_playlist.Current == null)
                {
                    RefreshNowPlaying();
                }

                SetPausedState(false);
                DisableScrubBar();
                RefreshNowPlaying();
            }
        }

        private void PlayQueue_MouseDown(object sender, MouseEventArgs e)
        {
            lock (_mediaActionLockObject)
            {
                _playlist.Position = _playListView.SelectedIndex;
                _audioServer.Stop(true);
                _audioServer.PlayAsync(_playlist.Current, TimeSpan.Zero, _preBufferSize);

                SetPausedState(false);
                DisableScrubBar();
                RefreshNowPlaying();
            }
        }

        private void RefreshNowPlaying()
        {
            if (_playlist.Current != null)
            {
                _audioServer.ChangeNowPlayingInfo("Now playing: " + new FileInfo(_playlist.Current).Name);
            }
            else
            {
                _audioServer.ChangeNowPlayingInfo(Program.NoAudioPlaying);
            }

            _playListView.Invoke(new Action(() =>
                    {
                        if (_playlist.Position < 0 || _playlist.Position >= _playListView.Items.Count) { return; }
                        _playListView.SelectedIndex = _playlist.Position;
                    }));
        }

        private void ShuffleBtnClicked(object sender, EventArgs e)
        {
            _playListView.Enabled = false;

            _playlist.Shuffle();
            _playListView.Items.Clear();
            foreach (string trackFileNames in _playlist.Tracks)
            {
                _playListView.Items.Add(new FileInfo(trackFileNames).Name);
            }

            _playListView.Enabled = true;
        }

        private void VolumeControlBar_Scroll(object sender, EventArgs e)
        {
            _audioServer.ChangeVolume(_volumeControlBar.Value / 100f);

            _audioLevelsImg.BackgroundImage = _volumeControlBar.Value switch
            {
                < 33 => Properties.Resources.volume_off_8x,
                < 66 => Properties.Resources.volume_low_8x,
                _ => Properties.Resources.volume_high_8x
            };
        }

        private void SearchSubFoldersBtn_Click(object sender, EventArgs e) => _searchSubFoldersBtn.Checked = !_searchSubFoldersBtn.Checked;

        private void RerouteAudioBtn_Click(object sender, EventArgs e)
        {
            ChangeGUIState(GUIState.AudioRerouterOpened);

            var routerForm = new AudioRerouter(_audioServer) { Owner = this };
            routerForm.FormClosed += (o, e) =>
            {
                ChangeGUIState(GUIState.ServerEntryClosed);
                _audioServer.ChangeNowPlayingInfo(Program.NoAudioPlaying);
            };
            routerForm.Show(this);
        }

        private void TrackElapsedTimeTicker_Tick(object sender, EventArgs e)
        {
            if (_manualScrubbing)
            {
                return;
            }

            TimeSpan currentTrackElapsedTime = _audioServer.CurrentTrackElapsedTime;
            if (currentTrackElapsedTime == TimeSpan.Zero)
            {
                if (_trackElapsedTimeLbl.Enabled)
                {
                    _trackElapsedTimeLbl.Enabled = false;
                    _trackElapsedTimeLbl.Text = "00:00";
                }
                return;
            }

            if (!_trackElapsedTimeLbl.Enabled)
            {
                _trackElapsedTimeLbl.Enabled = true;
            }

            UpdateScrubBarAndText(currentTrackElapsedTime);

            _trackScrubBar.Value = Math.Min((int)currentTrackElapsedTime.TotalMilliseconds, _trackScrubBar.Maximum);
        }

        private void UpdateScrubBarAndText(TimeSpan time)
        {
            if (time.Hours > 0)
            {
                _trackElapsedTimeLbl.Text = time.ToString("hh':'mm':'ss");
            }
            else
            {
                _trackElapsedTimeLbl.Text = time.ToString("mm':'ss");
            }
        }

        private bool _manualScrubbing = false;
        private int _manualScrubPosition = -1;
        private void TrackScrubBar_Scroll(object sender, EventArgs e)
        {
            _manualScrubPosition = _trackScrubBar.Value;
            UpdateScrubBarAndText(TimeSpan.FromMilliseconds(_manualScrubPosition));
        }

        private void TrackScrubBar_MouseDown(object sender, MouseEventArgs e)
        {
            _manualScrubbing = true;
        }

        private void TrackScrubBar_MouseUp(object sender, MouseEventArgs e)
        {
            DisableScrubBar(false);
            _manualScrubbing = false;
            lock (_mediaActionLockObject)
            {
                _audioServer.Stop(false);
                _audioServer.PlayAsync(_playlist.Current, TimeSpan.FromMilliseconds(_manualScrubPosition), _preBufferSize);
            }
        }
    }

    public class FormLogger : ILogger
    {
        public void Log(string message, ILogger.LogLevel severity = ILogger.LogLevel.Information)
        {
            MessageBox.Show(text: message, caption: severity.ToString());
        }
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message, ILogger.LogLevel severity = ILogger.LogLevel.Information)
        {
            Debug.WriteLine($"{severity}: {message}");
        }
    }
}

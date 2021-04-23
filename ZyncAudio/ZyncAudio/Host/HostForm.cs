﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ZyncAudio.Host;
using System.Windows.Forms;
using Zintom.StorageFacility;
using ZyncAudio.Extensions;
using System.Drawing;
using System.Diagnostics.CodeAnalysis;

namespace ZyncAudio
{
    public partial class HostForm : Form
    {
        private Server _server;

        private AudioServer _audioServer;

        private ILogger _logger;

        private readonly Playlist _playlist = new();

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

            _audioServer.PlaybackStarted += RefreshNowPlaying;
            _audioServer.PlaybackStoppedNaturally += PlaybackStoppedNaturally;

            var settings = Storage.GetStorage(Program.SettingsFile);
            _volumeControlBar.Value = settings.Integers.GetValue("volumeSliderValue", 100);
            VolumeControlBar_Scroll(null!, null!);
            _searchSubFoldersBtn.Checked = settings.Booleans.GetValue("searchSubFoldersOnLoadFolder", false);
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

        private bool _paused = true;
        private long _pausedOnByte = 0L;
        private readonly object _mediaActionLockObject = new();
        private void PlayBtn_Click(object sender, EventArgs e)
        {
            lock (_mediaActionLockObject)
            {
                if (_paused)
                {
                    _audioServer.PlayAsync(_playlist.Current, _pausedOnByte);

                    SetPausedState(false, 0L);
                }
                else
                {
                    SetPausedState(true, _audioServer.CurrentTrackPositionBytes);
                    _audioServer.Stop();
                }
            }
        }

        private void SetPausedState(bool paused, long pausedBytePosition = 0L)
        {
            if (paused)
            {
                _paused = true;
                _pausedOnByte = pausedBytePosition;
                _playBtn.BackgroundImage = Properties.Resources.media_play_8x;
                _toolTipProvider.SetToolTip(_playBtn, "Plays the current track.");
            }
            else
            {
                _paused = false;
                _pausedOnByte = 0L;
                _playBtn.BackgroundImage = Properties.Resources.media_pause_8x;
                _toolTipProvider.SetToolTip(_playBtn, "Pauses the current track.");
            }
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            lock (_mediaActionLockObject)
            {
                _audioServer.Stop();

                RefreshNowPlaying();
                SetPausedState(true);

                _audioServer.ChangeNowPlayingInfo(Program.NoAudioPlaying);
            }
        }

        private void CloseEntryBtn_Click(object sender, EventArgs e)
        {
            _server.StopAccepting();
            _closeEntryBtn.Enabled = false;

            _playBtn.Enabled = true;
            _stopBtn.Enabled = true;
            _previousBtn.Enabled = true;
            _nextBtn.Enabled = true;
            _playListView.Enabled = true;
            _loadFolderBtn.Enabled = true;
            _unloadPlaylistBtn.Enabled = true;
            _shuffleBtn.Enabled = true;
            _searchSubFoldersBtn.Enabled = true;
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
            _audioServer.Stop();
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
                        _playlist.Add(file);
                        _playListView.Items.Add(new FileInfo(file).Name);
                    }
                }
            }
        }

        private void UnloadItems_Click(object sender, EventArgs e)
        {
            _playlist.Clear();
            _playListView.Items.Clear();
        }

        private void PlaybackStoppedNaturally()
        {
            _playlist.MoveNext();
            _audioServer.PlayAsync(_playlist.Current);

            RefreshNowPlaying();
        }

        private void NextBtn_Click(object sender, EventArgs e)
        {
            lock (_mediaActionLockObject)
            {
                _audioServer.Stop();
                _playlist.MoveNext();
                _audioServer.PlayAsync(_playlist.Current);

                if (_playlist.Current == null)
                {
                    RefreshNowPlaying();
                }

                SetPausedState(false);
            }
        }

        private void PreviousBtn_Click(object sender, EventArgs e)
        {
            lock (_mediaActionLockObject)
            {
                _audioServer.Stop();
                _playlist.MovePrevious();
                _audioServer.PlayAsync(_playlist.Current);

                if (_playlist.Current == null)
                {
                    RefreshNowPlaying();
                }

                SetPausedState(false);
            }
        }

        private void PlayQueue_MouseDown(object sender, MouseEventArgs e)
        {
            lock (_mediaActionLockObject)
            {
                _playlist.Position = _playListView.SelectedIndex;
                _audioServer.Stop();
                _audioServer.PlayAsync(_playlist.Current);

                SetPausedState(false);
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

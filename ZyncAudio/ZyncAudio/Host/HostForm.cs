using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ZyncAudio.Host;
using System.Windows.Forms;

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
            ClientListView.Columns.Add("address", "Address", 120);
            ClientListView.Columns.Add("ping", "Ping", 50);

            _logger = new ConsoleLogger();
            _server = new Server(_logger);
            _audioServer = new AudioServer(_server);

            _server.Open(IPAddress.Any, 60759);
            _server.ClientConnected = ClientConnected;
            _server.ClientDisconnected = ClientDisconnected;

            _audioServer.PlaybackStarted += RefreshNowPlaying;
            _audioServer.PlaybackStoppedNaturally += PlaybackStoppedNaturally;
        }

        private void ClientConnected(Socket client)
        {
            Invoke(new Action(() =>
            {
                CloseEntryBtn.Enabled = true;

                ClientListView.Items.Add(new ListViewItem(new string[] { client.RemoteEndPoint?.ToString() ?? "null", "0ms" }));
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

        private void PlayBtn_Click(object sender, EventArgs e)
        {
            _audioServer.PlayAsync(_playlist.Current);
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            _audioServer.Stop();

            RefreshNowPlaying();
        }

        private void CloseEntryBtn_Click(object sender, EventArgs e)
        {
            _server.StopAccepting();
            CloseEntryBtn.Enabled = false;

            PlayBtn.Enabled = true;
            NextBtn.Enabled = true;
            PreviousBtn.Enabled = true;
            StopBtn.Enabled = true;
            PlayQueue.Enabled = true;
            LoadFolderBtn.Enabled = true;
            UnloadItems.Enabled = true;
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
        }

        private void LoadFolderBtn_Click(object sender, EventArgs e)
        {
            using (var folderBrowser = new FolderBrowserDialog())
            {
                var result = folderBrowser.ShowDialog();
                if (result != DialogResult.OK) { return; }

                string folder = folderBrowser.SelectedPath;

                foreach (var file in Directory.GetFiles(folder))
                {
                    if (file.EndsWith(".wav")
                        || file.EndsWith(".mp3")
                        || file.EndsWith(".flac"))
                    {
                        _playlist.Add(file);
                        PlayQueue.Items.Add(new FileInfo(file).Name);
                    }
                }
            }
        }

        private void UnloadItems_Click(object sender, EventArgs e)
        {
            _playlist.Clear();
            PlayQueue.Items.Clear();
        }

        private void PlaybackStoppedNaturally()
        {
            _playlist.MoveNext();
            _audioServer.PlayAsync(_playlist.Current);
        }

        private void NextBtn_Click(object sender, EventArgs e)
        {
            _audioServer.Stop();
            _playlist.MoveNext();
            _audioServer.PlayAsync(_playlist.Current);
        }

        private void PreviousBtn_Click(object sender, EventArgs e)
        {
            _audioServer.Stop();
            _playlist.MovePrevious();
            _audioServer.PlayAsync(_playlist.Current);
        }

        private void PlayQueue_MouseDown(object sender, MouseEventArgs e)
        {
            _playlist.Position = PlayQueue.SelectedIndex;
            _audioServer.Stop();
            _audioServer.PlayAsync(_playlist.Current);
        }

        private void RefreshNowPlaying()
        {
            PlayQueue.Invoke(new Action(() =>
            {
                if (_playlist.Position < 0 || _playlist.Position >= PlayQueue.Items.Count) { return; }
                PlayQueue.SelectedIndex = _playlist.Position;
            }));
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

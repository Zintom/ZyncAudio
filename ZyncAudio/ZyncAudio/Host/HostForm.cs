﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZyncAudio
{
    public partial class HostForm : Form
    {
        private Server _server;

        private AudioServer _audioServer;

        private ILogger _logger;

        private CancellationTokenSource? _cancelPlaybackSource;

        private Queue<string> _playingQueue = new();

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

        private async void PlayBtn_Click(object sender, EventArgs e)
        {
            //_audioServer.Play(@"D:\All Files\Music\04. Mike Ault - Flying Forever (feat. Morgan Perry).flac");

            _cancelPlaybackSource?.Cancel();
            _cancelPlaybackSource = new CancellationTokenSource();
            await _audioServer.PlayQueue(_playingQueue, _cancelPlaybackSource.Token);
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            _audioServer.Stop();
            _cancelPlaybackSource?.Cancel();
        }

        private void CloseEntryBtn_Click(object sender, EventArgs e)
        {
            _server.StopAccepting();
            CloseEntryBtn.Enabled = false;

            PlayBtn.Enabled = true;
            StopBtn.Enabled = true;
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
                        _playingQueue.Enqueue(file);
                        PlayQueue.Items.Add(new FileInfo(file).Name);
                    }
                }
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

﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using Zintom.StorageFacility;

namespace ZyncAudio
{
    public partial class ClientForm : Form
    {
        Client _client;
        AudioClient _audioClient;

        ILogger _logger;

        HostForm? _host;

        public ClientForm()
        {
            InitializeComponent();
            _nowPlayingBarText.Text = Program.NoAudioPlaying;

            _logger = new ConsoleLogger();
            _client = new Client();
            _client.SocketError = HandleSocketError;
            _audioClient = new AudioClient(_client, _logger);
            _audioClient.TrackInformationChanged += UpdateTrackInformationText;

            _ipAddressInputBox.Text = Storage.GetStorage(Program.SettingsFile).Strings.GetValue("lastConnectedIpAddress", "127.0.0.1");
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
        }

        private void HostButton_Click(object sender, EventArgs e)
        {
            _hostBtn.Enabled = false;

            _host = new HostForm();
            _host.Show();
        }

        private void ClientForm_Activated(object sender, EventArgs e)
        {

        }

        public void HandleSocketError(SocketException e, Socket _)
        {
            MessageBox.Show(caption: nameof(SocketException), text: e.Message);
        }

        private bool _connectToggle = false;
        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            if (_connectToggle)
            {
                if (_client.Disconnect())
                {
                    _connectToggle = !_connectToggle;
                    _connectBtn.Text = "Connect";
                }

                _audioClient.Disconnect();
            }
            else
            {
                if (_client.Connect(IPAddress.Parse(_ipAddressInputBox.Text), 60759))
                {
                    _connectToggle = !_connectToggle;
                    _connectBtn.Text = "Disconnect";
                }
            }
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _audioClient.Disconnect();
            Storage.GetStorage(Program.SettingsFile).Edit().Commit();
        }

        private void IPAddressInputBox_TextChanged(object sender, EventArgs e)
        {
            Storage.GetStorage(Program.SettingsFile).Edit().PutValue("lastConnectedIpAddress", _ipAddressInputBox.Text);
        }

        private void UpdateTrackInformationText(string nowPlaying)
        {
            _nowPlayingBarText.Invoke(new Action(() =>
            {
                if (nowPlaying == Program.NoAudioPlaying)
                {
                    // Lock the now playing text to the left of the bar
                    _nowPlayingBarText.Left = 3;
                    _nowPlayingBarAnimator.Stop();
                }
                else
                {
                    // Place the now playing text a little bit inwards from the left
                    // so that the user can see the start of the text before it animates off screen.
                    _nowPlayingBarText.Left = 40;
                    _nowPlayingBarAnimator.Start();
                }

                _nowPlayingBarText.Text = nowPlaying;
            }));
        }

        private void NowPlayingBarTick(object sender, EventArgs e)
        {
            if (_nowPlayingBarText.Right < 0)
            {
                _nowPlayingBarText.Left = _nowPlayingBarText.Parent.Width;
            }

            if (_nowPlayingBarText.Left > _nowPlayingBarText.Parent.Width)
            {
                _nowPlayingBarText.Left = _nowPlayingBarText.Parent.Width;
            }

            //_nowPlayingBarText.Left = Math.Clamp(_nowPlayingBarText.Left, 0 - _nowPlayingBarText.Width, _nowPlayingBarText.Parent.Width);

            _nowPlayingBarText.Left -= 2;
        }
    }
}

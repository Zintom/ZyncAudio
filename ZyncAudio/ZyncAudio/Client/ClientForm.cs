using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            _logger = new ConsoleLogger();
            _client = new Client();
            _client.SocketError = HandleSocketError;
            _audioClient = new AudioClient(_client, _logger);
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
        }

        private void HostButton_Click(object sender, EventArgs e)
        {
            HostBtn.Enabled = false;

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
                    ConnectBtn.Text = "Connect";
                }
            }
            else
            {
                if (_client.Connect(IPAddress.Parse(IPAddressInputBox.Text), 60759))
                {
                    _connectToggle = !_connectToggle;
                    ConnectBtn.Text = "Disconnect";
                }
            }
        }
    }
}

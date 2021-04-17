using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
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

        public ClientForm()
        {
            InitializeComponent();

            HostForm host = new HostForm();
            host.Show();

            _logger = new ConsoleLogger();
            _client = new Client();
            _audioClient = new AudioClient(_client, _logger);

            _client.Connect(IPAddress.Loopback, 60759);
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
        }
    }
}

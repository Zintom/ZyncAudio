using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZyncAudio
{
    public partial class HostForm : Form
    {
        private Server _server;
        private AudioServer _audioServer;

        private ILogger _logger;

        public HostForm()
        {
            InitializeComponent();

            _logger = new ConsoleLogger();
            _server = new Server(_logger);
            _audioServer = new AudioServer(_server);

            _server.Open(IPAddress.Any, 60759);
            _server.ClientConnected = ClientConnected;
        }

        private void ClientConnected()
        {
            _audioServer.PlaySong(@"D:\All Files\Music\04. Mike Ault - Flying Forever (feat. Morgan Perry).flac");
        }

        private void HostForm_Load(object sender, EventArgs e)
        {

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

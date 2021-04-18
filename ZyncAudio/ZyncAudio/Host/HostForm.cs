using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
                foreach(ListViewItem item in ClientListView.Items)
                {
                    if(item.Text == remoteAddress)
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
            _audioServer.Play(@"D:\All Files\Music\04. Mike Ault - Flying Forever (feat. Morgan Perry).flac");
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            _audioServer.Stop();
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
            foreach(var pair in _audioServer.Pinger.PingStatistics)
            {
                string ping = pair.Value + "ms";
                string address = pair.Key.RemoteEndPoint?.ToString() ?? "null";

                foreach(ListViewItem item in ClientListView.Items)
                {
                    if(item.Text == address)
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

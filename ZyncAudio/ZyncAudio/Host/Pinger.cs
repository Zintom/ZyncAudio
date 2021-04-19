using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZyncAudio.Host
{
    public class Pinger
    {

        private bool _running = false;

        private readonly ISocketServer _socketServer;

        private Dictionary<Socket, Stopwatch> _pingWatches = new();

        public Dictionary<Socket, long> PingStatistics { get; } = new();

        /// <summary>
        /// Set when we have received at least one ping from all connected clients.
        /// </summary>
        public ManualResetEvent HasResponseFromAll { get; } = new ManualResetEvent(false);

        public long HighestPingClient
        {
            get
            {
                long highestPing = 0;
                foreach (var latency in PingStatistics)
                {
                    if (latency.Value > highestPing)
                    {
                        highestPing = latency.Value;
                    }
                }
                return highestPing;
            }
        }

        public Pinger(ISocketServer socketServer)
        {
            _socketServer = socketServer;
        }

        /// <summary>
        /// Begin pinging clients connected to the <see cref="ISocketServer"/>
        /// </summary>
        public void Start()
        {
            HasResponseFromAll.Reset();

            _running = true;
            Task.Run(PingClients);
        }

        /// <summary>
        /// Stop pinging clients.
        /// </summary>
        public void Stop()
        {
            _running = false;
        }

        /// <summary>
        /// Call when a response to a ping is received.
        /// </summary>
        public void PingResponse(byte[] _, Socket client)
        {
            var watch = _pingWatches[client];
            watch.Stop();

            PingStatistics[client] = watch.ElapsedMilliseconds;
            watch.Reset();

            if (PingStatistics.Keys.Count >= _socketServer.Clients.Count)
            {
                HasResponseFromAll.Set();
            }
        }

        /// <summary>
        /// Pings clients periodically to get their latency times.
        /// </summary>
        private async Task PingClients()
        {
            while (_running)
            {
                foreach (var client in _socketServer.Clients)
                {
                    // Initialize a stopwatch for this client if it does not exist.
                    if (!_pingWatches.TryGetValue(client, out Stopwatch? watch))
                    {
                        watch = new Stopwatch();
                        _pingWatches.Add(client, watch);
                    }

                    // If we are still waiting for a response from the client then pospone till the next check.
                    if (watch.ElapsedMilliseconds > 0)
                    {
                        continue;
                    }

                    watch.Start();

                    _socketServer.Send(BitConverter.GetBytes((int)(MessageIdentifier.Request | MessageIdentifier.Ping)), client);
                }

                await Task.Delay(1000);
            }
        }

    }
}

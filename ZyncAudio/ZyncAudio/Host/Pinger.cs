using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ZyncAudio.Host
{
    public class Pinger
    {

        private bool _running = false;

        private readonly ISocketServer _socketServer;

        private readonly Dictionary<Socket, Stopwatch> _pingWatches = new();

        public Dictionary<Socket, long> PingStatistics { get; } = new();

        /// <summary>
        /// Set when we have received at least one ping from all connected clients.
        /// </summary>
        public ManualResetEvent HasResponseFromAll { get; } = new ManualResetEvent(false);

        /// <summary>
        /// Gets the ping time of the slowest client.
        /// </summary>
        public long HighestPing
        {
            get
            {
                long highestPing = 0;
                foreach (var ping in PingStatistics)
                {
                    if (ping.Value > highestPing)
                    {
                        highestPing = ping.Value;
                    }
                }
                return highestPing;
            }
        }

        /// <summary>
        /// Gets the latency time of the slowest client.
        /// </summary>
        public long HighestLatency
        {
            get => HighestPing / 2;
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
            ThreadPool.QueueUserWorkItem(PingClients);
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
        public void PingResponseReceived(byte[] _, Socket client)
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
        private async void PingClients(object? _)
        {
            while (_running)
            {
                // Make a local copy of the client list in case it is modified during enumeration.
                Socket[] clients = _socketServer.Clients.ToArray();

                foreach (var client in clients)
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

                    _socketServer.Send(BitConverter.GetBytes((int)(MessageIdentifier.Request | MessageIdentifier.Ping | MessageIdentifier.ProcessImmediately)), client);
                }

                await Task.Delay(1000).ConfigureAwait(false);
            }
        }

    }
}

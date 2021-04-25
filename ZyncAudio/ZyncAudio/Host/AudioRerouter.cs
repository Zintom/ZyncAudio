using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace ZyncAudio.Host
{
    public partial class AudioRerouter : Form
    {
        AudioServer _audioServer;

        private readonly ManualResetEvent _rerouterFinished = new ManualResetEvent(true);
        private readonly ManualResetEvent _exitRerouterThread = new ManualResetEvent(false);

        private readonly ManualResetEvent _exitSilenceThread = new(false);
        private readonly object _playSilenceLocker = new();

        // We have to manually update these as the re-routing thread cannot interact with GUI controls.
        private int _sourcesSize;
        private string? _selectedSourceID;

        public AudioRerouter(AudioServer audioServer)
        {
            InitializeComponent();
            _audioServer = audioServer;

            RefreshSources(null, null);
        }

        private void Audio_Rerouter_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Plays silence on the audio device which matches the given <paramref name="mmDeviceId"/>.
        /// </summary>
        private void PlayAmbientSilenceAsync(WaveFormat waveFormat, string mmDeviceId)
        {
            lock (_playSilenceLocker)
            {
                _exitSilenceThread.Set();
                _exitSilenceThread.Reset();

                new Thread(() =>
                {
                    var silenceProvider = new SilenceProvider(waveFormat);

                    using (var wasapiOut = new WasapiOut(GetMMDeviceByID(mmDeviceId), AudioClientShareMode.Shared, false, 250))
                    {
                        wasapiOut.Init(silenceProvider);
                        wasapiOut.Play();

                        _exitSilenceThread.WaitOne();
                    }
                })
                {
                    Priority = ThreadPriority.BelowNormal,
                    IsBackground = true
                }.Start();
            }
        }

        /// <summary>
        /// Gets a thread specific <see cref="MMDevice"/> by its ID.
        /// </summary>
        /// <remarks>An <see cref="MMDevice"/> cannot be used cross-thread.</remarks>
        private static MMDevice? GetMMDeviceByID(string targetID)
        {
            foreach (var device in new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                if (device.ID == targetID)
                {
                    return device;
                }
            }

            return null;
        }

        /// <summary>
        /// Begins re-routing audio from the selected playback device.
        /// </summary>
        private void ExecuteRerouteAsync()
        {
            _audioServer.Stop();
            _audioServer.ChangeNowPlayingInfo("Streaming live audio from the host machine.");

            new Thread(() =>
            {
                _rerouterFinished.Reset();

                if (_sourcesSize == 0 || _selectedSourceID == null)
                {
                    _rerouterFinished.Set();
                    return;
                }

                // We have to re-establish the collection of devices
                // as the COM component is retrieved on a per thread basis.
                MMDevice? targetAudioDevice = GetMMDeviceByID(_selectedSourceID);

                if (targetAudioDevice == null)
                {
                    _rerouterFinished.Set();
                    return;
                }

                var capture = new WasapiLoopbackCapture(targetAudioDevice);

                // We HAVE to play silence on the audio device we are recording
                // otherwise WASAPI will stop sending data when no tracks are playing.
                // This means clients will get out of sync due to there being no constant stream of data.
                PlayAmbientSilenceAsync(capture.WaveFormat, _selectedSourceID);

                var bufferedProvider = new BufferedWaveProvider(capture.WaveFormat)
                {
                    BufferDuration = TimeSpan.FromSeconds(30)
                };

                long timeSinceLastUpdate = 0;
                capture.DataAvailable += (sender, eventArgs) =>
                {
                    bufferedProvider.AddSamples(eventArgs.Buffer, 0, eventArgs.BytesRecorded);

                    long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    if (now - timeSinceLastUpdate > 1000L)
                    {
                        timeSinceLastUpdate = now;
                        Debug.WriteLine("Audio Rerouter bufferred: " + bufferedProvider.BufferedDuration);
                    }
                };

                // Begin capturing data from the audio device.
                capture.StartRecording();
                _audioServer.PlayLiveAudioAsync(bufferedProvider);

                // Re-route until we are told to stop.
                _exitRerouterThread.WaitOne();

                _audioServer.Stop();

                capture.StopRecording();
                capture.Dispose();

                _rerouterFinished.Set();
            })
            {
                IsBackground = true
            }.Start();
        }

        private void RefreshSources(object? sender, EventArgs? e)
        {
            _audioDeviceSourceBox.Items.Clear();
            _audioDeviceSourceBox.Items.Add("None");

            var sources = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            foreach (var device in sources)
            {
                _audioDeviceSourceBox.Items.Add(device);
            }

            _sourcesSize = sources.Count;

            if(_audioDeviceSourceBox.SelectedIndex == -1)
            {
                _audioDeviceSourceBox.SelectedIndex = 0;
            }
        }

        private void AudioDeviceSourceBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If "None" then just null the selected source and return.
            if (_audioDeviceSourceBox.SelectedIndex == 0)
            {
                _exitRerouterThread.Set();
                _exitRerouterThread.Reset();
                _rerouterFinished.WaitOne();

                _selectedSourceID = null;
                _audioServer.Stop();
                return;
            }

            _selectedSourceID = ((MMDevice)_audioDeviceSourceBox.SelectedItem).ID;

            // Inform the rerouting thread to stop
            _exitRerouterThread.Set();
            _exitRerouterThread.Reset();

            // Wait for the rerouting thread to stop.
            _rerouterFinished.WaitOne();

            // Begin a new route on the newly selected device.
            ExecuteRerouteAsync();
        }

        private void AudioRerouter_FormClosing(object sender, FormClosingEventArgs e)
        {
            _exitRerouterThread.Set();
            _exitSilenceThread.Set();
        }
    }
}

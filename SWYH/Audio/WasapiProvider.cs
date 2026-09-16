/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: WasapiProvider.cs
 *	 Web site: https://www.streamwhatyouhear.com
 *	 Copyright (C) 2012-2026 - Sebastien Warin <https://sebastien.warin.fr> and others
 *
 *   This file is part of Stream What Your Hear.
 *	 
 *	 Stream What Your Hear is free software: you can redistribute it and/or modify
 *	 it under the terms of the GNU General Public License as published by
 *	 the Free Software Foundation, either version 2 of the License, or
 *	 (at your option) any later version.
 *	 
 *	 Stream What Your Hear is distributed in the hope that it will be useful,
 *	 but WITHOUT ANY WARRANTY; without even the implied warranty of
 *	 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *	 GNU General Public License for more details.
 *	 
 *	 You should have received a copy of the GNU General Public License
 *	 along with Stream What Your Hear. If not, see <http://www.gnu.org/licenses/>.
 */

namespace SWYH.Audio
{
    using NAudio.Lame;
    using NAudio.Wave;
    using NAudio.CoreAudioApi;
    using System;
    using System.Threading;

    internal class WasapiProvider : IDisposable
    {
        private byte[] buffer = new byte[1024];

        private volatile bool isRunning = true;
        private volatile bool hasMP3Clients = false;
        private volatile bool hasPCMClients = false;

        private long lastCaptureTime = Environment.TickCount64;
        private readonly object captureLock = new object();
        private readonly ManualResetEventSlim stopping = new ManualResetEventSlim();
        private Thread waveProcessorThread;
        private Thread silenceGeneratorThread;
        private MMDevice captureDevice;
        private WaveStream rawWave16b;
        private int disposed;

        private WasapiRecorder loopbackWaveIn = null;
        private PipeStream recordingStream = null;
        private WaveStream rawConvertedStream = null;
        private WaveStream pcmStream = null;
        private LameMP3FileWriter mp3Writer = null;

        public PipeStream LoopbackMp3Stream { get; private set; }
        public PipeStream LoopbackL16Stream { get; private set; }

        public WasapiProvider()
        {
            // Init Pipes
            this.recordingStream = new PipeStream();
            this.LoopbackMp3Stream = new PipeStream();
            this.LoopbackL16Stream = new PipeStream();

            // Init Wave Processor thread
            this.waveProcessorThread = new Thread(new ThreadStart(this.waveProcessor)) { Priority = ThreadPriority.Highest };

            // Feeds silence to the clients when Windows delivers no loopback data (nothing is playing)
            this.silenceGeneratorThread = new Thread(new ThreadStart(this.silenceGenerator)) { IsBackground = true };

            // Init capture audio device
            try
            {
                using var enumerator = new MMDeviceEnumerator();
                string captureDeviceID = SWYH.Properties.Settings.Default.AudioDevice;
                if (captureDeviceID != "")
                {
                    foreach (var wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active))
                    {
                        if (wasapi.ID == captureDeviceID)
                        {
                            captureDevice = wasapi;
                        }
                        else
                        {
                            wasapi.Dispose();
                        }
                    }
                }
                if (captureDevice == null)
                {
                    captureDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                }

                // Keep the converter and silence generator on the same explicit float format.
                using var audioClient = captureDevice.CreateAudioClient();
                var mixFormat = audioClient.MixFormat;
                var captureFormat = WaveFormat.CreateIeeeFloatWaveFormat(mixFormat.SampleRate, mixFormat.Channels);
                var builder = new WasapiRecorderBuilder().WithDevice(captureDevice).WithFormat(captureFormat);
                if (captureDevice.DataFlow == DataFlow.Render)
                {
                    builder.WithLoopbackCapture();
                }
                this.loopbackWaveIn = builder.Build();
                this.loopbackWaveIn.DataAvailable += this.loopbackWaveIn_DataAvailable;
                this.rawWave16b = new Wave32To16Stream(new RawSourceWaveStream(this.recordingStream, this.loopbackWaveIn.WaveFormat));

                // Convert Raw Wav to PCM with audio format in settings
                var audioFormat = AudioSettings.GetAudioFormat();
                if (rawWave16b.WaveFormat.SampleRate == audioFormat.SampleRate
                    && rawWave16b.WaveFormat.BitsPerSample == audioFormat.BitsPerSample
                    && rawWave16b.WaveFormat.Channels == audioFormat.Channels)
                {
                    // No conversion !
                    this.rawConvertedStream = null;
                    this.pcmStream = rawWave16b;
                }
                else
                {
                    // Resampler
                    this.rawConvertedStream = new WaveProviderToWaveStream(new MediaFoundationResampler(rawWave16b, audioFormat));
                    this.pcmStream = rawConvertedStream;
                }

                // Init MP3 Encoder
                this.mp3Writer = new LameMP3FileWriter(this.LoopbackMp3Stream, pcmStream.WaveFormat, AudioSettings.GetMP3Bitrate());

                // Start Recording
                this.loopbackWaveIn.StartRecording();

                // Start Wave Processor thread
                waveProcessorThread.Start();
                silenceGeneratorThread.Start();
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref this.disposed, 1) != 0) return;
            this.isRunning = false;
            this.stopping.Set();
            // Release every possible blocked pipe operation before joining capture/workers.
            this.recordingStream?.CompleteWriting();
            this.LoopbackMp3Stream?.CompleteWriting();
            this.LoopbackL16Stream?.CompleteWriting();
            this.loopbackWaveIn?.Dispose();
            if (this.waveProcessorThread?.IsAlive == true) this.waveProcessorThread.Join();
            if (this.silenceGeneratorThread?.IsAlive == true) this.silenceGeneratorThread.Join();
            this.mp3Writer?.Dispose();
            this.rawConvertedStream?.Dispose();
            this.rawWave16b?.Dispose();
            this.recordingStream?.Dispose();
            this.LoopbackMp3Stream?.Dispose();
            this.LoopbackL16Stream?.Dispose();
            this.captureDevice?.Dispose();
            this.stopping.Dispose();
        }

        internal void UpdateClientsList(int mp3ClientsCount, int pcmClientsCount)
        {
            this.hasMP3Clients = mp3ClientsCount > 0;
            this.hasPCMClients = pcmClientsCount > 0;
        }

        private void waveProcessor()
        {
            while (this.isRunning)
            {
                if (this.hasMP3Clients || this.hasPCMClients)
                {
                    try
                    {
                        int readBytes = this.pcmStream.Read(buffer, 0, buffer.Length);
                        if (readBytes > 0)
                        {
                            if (this.hasMP3Clients)
                            {
                                // MP3 stream
                                this.mp3Writer.Write(this.buffer, 0, readBytes);
                            }
                            if (this.hasPCMClients)
                            {
                                // L16 stream
                                if (BitConverter.IsLittleEndian)
                                {
                                    for (int i = 0; i < readBytes; i += 2)
                                    {
                                        Array.Reverse(buffer, i, 2);
                                    }
                                }
                                this.LoopbackL16Stream.Write(buffer, 0, readBytes);
                            }
                        }
                    }
                    catch
                    {
                        if (this.isRunning)
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    this.stopping.Wait(1);
                }
            }
        }

        private void silenceGenerator()
        {
            const int SilenceChunkMs = 100;
            var format = this.loopbackWaveIn.WaveFormat;
            byte[] silence = new byte[(format.SampleRate * SilenceChunkMs / 1000) * format.BlockAlign];
            while (!this.stopping.Wait(SilenceChunkMs))
            {
                lock (this.captureLock)
                {
                    if (this.isRunning && (this.hasMP3Clients || this.hasPCMClients)
                        && Environment.TickCount64 - this.lastCaptureTime > SilenceChunkMs * 1.5)
                    {
                        this.recordingStream.Write(silence);
                    }
                }
            }
        }

        private void loopbackWaveIn_DataAvailable(ReadOnlySpan<byte> buffer, AudioClientBufferFlags flags,
            long devicePosition, long qpcPosition)
        {
            lock (this.captureLock)
            {
                if (!this.isRunning) return;
                if (!buffer.IsEmpty) this.lastCaptureTime = Environment.TickCount64;
                if (this.hasMP3Clients || this.hasPCMClients)
                {
                    // Copy during the callback: NAudio owns the span's native memory.
                    this.recordingStream.Write(buffer);
                }
            }
        }
    }
}

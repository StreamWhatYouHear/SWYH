/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: WasapiProvider.cs
 *	 Web site: http://www.streamwhatyouhear.com
 *	 Copyright (C) 2012-2015 - Sebastien Warin <http://sebastien.warin.fr>	   	
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
    using System;
    using System.Threading;
    using NAudio.Wave;
    using SWYH.Audio.Mp3;

    internal class WasapiProvider
    {
        private bool isRunning = true;
        private byte[] buffer = null;

        private WasapiLoopbackCapture loopbackWaveIn = null;
        private PipeStream recordingStream = null;
        private WaveStream rawConvertedStream = null;
        private WaveStream pcmStream = null;
        private AudioMp3Writer mp3Writer = null;

        public bool IsRecording { get; private set; }
        public PipeStream LoopbackMp3Stream { get; private set; }
        public PipeStream LoopbackL16Stream { get; private set; }

        public WasapiProvider()
        {
            // Init Pipe
            this.recordingStream = new PipeStream();
            this.LoopbackMp3Stream = new PipeStream();
            this.LoopbackL16Stream = new PipeStream();

            // Init Wave Processor thread
            Thread waveProcessorThread = new Thread(new ThreadStart(this.waveProcessor)) { Priority = ThreadPriority.Highest };

            // Init Wasapi Capture
            this.loopbackWaveIn = new WasapiLoopbackCapture();
            this.loopbackWaveIn.DataAvailable += new EventHandler<WaveInEventArgs>(this.loopbackWaveIn_DataAvailable);

            // Init Raw Wav (48kHz 16bit Stereo)
            WaveStream rawWave48k16b = new Wave32To16Stream(new RawSourceWaveStream(this.recordingStream, NAudio.Wave.WaveFormat.CreateIeeeFloatWaveFormat(this.loopbackWaveIn.WaveFormat.SampleRate, this.loopbackWaveIn.WaveFormat.Channels)));

            // Convert Wav to PCM with audio format in settings
            var audioFormat = AudioSettings.GetAudioFormat();
            if (this.loopbackWaveIn.WaveFormat.SampleRate == audioFormat.SampleRate
                && this.loopbackWaveIn.WaveFormat.BitsPerSample == audioFormat.BitsPerSample
                && this.loopbackWaveIn.WaveFormat.Channels == audioFormat.Channels)
            {
                this.rawConvertedStream = null;
                this.pcmStream = WaveFormatConversionStream.CreatePcmStream(rawWave48k16b);
            }
            else
            {
                this.rawConvertedStream = new WaveFormatConversionStream(AudioSettings.GetAudioFormat(), rawWave48k16b);
                this.pcmStream = WaveFormatConversionStream.CreatePcmStream(rawConvertedStream);
            }

            // Init MP3 Encoder
            var mp3WaveFormat = new Mp3.WaveFormat(pcmStream.WaveFormat.SampleRate, pcmStream.WaveFormat.BitsPerSample, pcmStream.WaveFormat.Channels);
            this.mp3Writer = new AudioMp3Writer(this.LoopbackMp3Stream, mp3WaveFormat, new BE_CONFIG(mp3WaveFormat, AudioSettings.GetMP3Bitrate()));

            // Init Buffer with OptimalBufferSize
            this.buffer = new byte[this.mp3Writer.OptimalBufferSize];

            // Start Recording
            this.loopbackWaveIn.StartRecording();

            // Start Wave Processor thread
            waveProcessorThread.Start();
        }

        public void StartRecording()
        {
            if (!this.IsRecording)
            {
                this.IsRecording = true;
            }
        }

        public void StopRecording()
        {
            if (this.IsRecording)
            {
                this.IsRecording = false;
            }
        }

        public void Dispose()
        {
            this.StopRecording();
            this.isRunning = false;
            Thread.Sleep(200);
            this.loopbackWaveIn.StopRecording();
            this.recordingStream.Flush();
            this.recordingStream.Dispose();
            if (this.rawConvertedStream != null)
            {
                this.rawConvertedStream.Flush();
                this.rawConvertedStream.Dispose();
            }
            this.LoopbackMp3Stream.Flush();
            this.LoopbackMp3Stream.Dispose();
            this.LoopbackL16Stream.Flush();
            this.LoopbackL16Stream.Dispose();
        }

        public void UpdateClientsList()
        {
            this.IsRecording = (App.CurrentInstance.swyhDevice.sessionMp3Streams.Count > 0 || App.CurrentInstance.swyhDevice.sessionPcmStreams.Count > 0);
        }

        private void waveProcessor()
        {
            while (this.isRunning)
            {
                if (this.IsRecording)
                {
                    int readBytes = this.pcmStream.Read(buffer, 0, buffer.Length);
                    if (readBytes > 0)
                    {
                        // MP3 stream
                        this.mp3Writer.Write(this.buffer, 0, readBytes);
                        // L16 stream
                        if (BitConverter.IsLittleEndian)
                        {
                            for (int i = 0; i < readBytes; i += 2)
                            {
                                Array.Reverse(buffer, i, 2);
                            }
                        }
                        this.LoopbackL16Stream.Write(buffer, 0, buffer.Length);
                    }
                }
                Thread.Sleep(1);
            }
        }

        private void loopbackWaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (this.IsRecording)
            {
                this.recordingStream.Write(e.Buffer, 0, e.BytesRecorded);
            }
        }
    }
}

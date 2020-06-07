using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace AudioLib
{
    public class Player
    {
        static WaveOut _waveOut = new WaveOut();

        public void PlayTone(double duration = 1, double freq = 1000, double volume = 0.5, int sampleRate = 48000)
        {
            int sampleCount = (sampleRate * duration).ToInt32();
            List<float> samples = new List<float>(sampleCount);
            var durationInRadians = 2 * Math.PI * (duration * freq);
            for (int i = 0; i < sampleCount; i++)
            {
                var phase = ((double)i / sampleCount) * durationInRadians;
                samples.Add((volume * Math.Sin(phase)).ToSingle());
            }

            Play(samples, sampleRate);
        }

        public void Play(IEnumerable<double> samples, int sampleRate = 48000)
        {
            Play(samples?.Select(s => s.ToSingle()), sampleRate);
        }

        public void Play(IEnumerable<float> samples, int sampleRate = 48000)
        {
            Stop();
            if (samples == null)
            {
                return;
            }

            List<byte> bytes = new List<byte>();
            samples.ForEach(s => bytes.AddRange(BitConverter.GetBytes(s)));

            IWaveProvider provider = new RawSourceWaveStream(
                         new MemoryStream(bytes.ToArray()), WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1));

            _waveOut.Init(provider);
            _waveOut.Play();
        }

        public void Stop()
        {
            _waveOut.Stop();
        }
    }
}

using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;

namespace AudioLib
{
    public static class AudioUtils
    {
        public static List<float> ReadSamples(string path, int sampleRate)
        {
            path = path ?? throw new ArgumentNullException();
            ThrowUtils.ThrowIf_FileNotExist(path);

            AudioFileReader reader = new AudioFileReader(path);
            ThrowUtils.ThrowIf_True(reader.WaveFormat.SampleRate != sampleRate,
                "reader.WaveFormat.SampleRate != sampleRate ({0} != {1})".Format(reader.WaveFormat.SampleRate, sampleRate));

            int totalSamples = (reader.TotalTime.TotalSeconds * sampleRate).ToInt32();
            reader.Volume = 1;
            float[] buff = new float[totalSamples];
            var count = reader.ToMono(1, 0).Read(buff, 0, totalSamples);

            return buff.ToList();
        }
    }
}

// Copyright (c) 2019 Lordmau5
using NAudio.Wave;
using NAudio.Vorbis;
using System;
using System.IO;
using System.Reflection;

namespace GtaChaos.Models.Utils
{
    public class AudioPlayer
    {
        public static readonly AudioPlayer INSTANCE = new AudioPlayer();

        private readonly string[] supportedFormats =
        {
            "mp3", "wav", "aac", "m4a"
        };

        private void PlayEmbeddedResource(string type, string path)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            Stream s = a.GetManifestResourceStream($"GtaChaos.Models.{type}.{path}.m4a");

            string fullPath = $"{type}/{path}";

            WaveStream stream = null;

            // ogg / Vorbis
            try
            {
                stream = new VorbisWaveReader($"{fullPath}.ogg");
            }
            catch { }

            // Iterate over supported formats
            if (stream == null)
            {
                foreach (string format in supportedFormats)
                {
                    try
                    {
                        stream = new MediaFoundationReader($"{fullPath}.{format}");

                        if (stream != null) break;
                    }
                    catch { }
                }
            }

            // Try embedded resources
            if (stream == null)
            {
                try
                {
                    stream = new StreamMediaFoundationReader(s);
                }
                catch { }
            }

            if (stream == null) return;

            WaveOutEvent outputDevice = new WaveOutEvent();
            outputDevice.Init(stream);

            outputDevice.Play();
        }

        public void PlayAudio(string res)
        {
            PlayEmbeddedResource("audio", res);
        }
    }
}

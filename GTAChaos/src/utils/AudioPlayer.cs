// Copyright (c) 2019 Lordmau5
using NAudio.Vorbis;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GTAChaos.Utils
{
    public class AudioPlayer
    {
        private class Audio
        {
            private readonly string[] supportedFormats =
            {
                "mp3", "wav", "aac", "m4a"
            };

            public event EventHandler<EventArgs> OnFinished;

            private string Path { get; }

            private DateTime Expiry { get; }

            public Audio(string path)
            {
                this.Path = path;
                this.Expiry = DateTime.Now.AddSeconds(30);
            }

            public void Play()
            {
                if (DateTime.Now > this.Expiry)
                {
                    OnFinished?.Invoke(this, EventArgs.Empty);
                    return;
                }

                Assembly a = Assembly.GetExecutingAssembly();
                Stream s = a.GetManifestResourceStream($"GTAChaos.assets.audio.{this.Path}.ogg");

                WaveStream stream = null;

                // ogg / Vorbis
                try
                {
                    stream = new VorbisWaveReader($"{this.Path}.ogg");
                }
                catch { }

                // Iterate over supported formats
                if (stream == null)
                {
                    foreach (string format in this.supportedFormats)
                    {
                        try
                        {
                            stream = new MediaFoundationReader($"{this.Path}.{format}");

                            if (stream != null)
                            {
                                break;
                            }
                        }
                        catch { }
                    }
                }

                // Try embedded resources
                if (stream == null)
                {
                    try
                    {
                        stream = new VorbisWaveReader(s);
                    }
                    catch { }
                }

                if (stream == null)
                {
                    return;
                }

                WaveOutEvent outputDevice = new();
                outputDevice.Init(stream);
                outputDevice.PlaybackStopped += (object sender, StoppedEventArgs e) => OnFinished?.Invoke(this, e);

                outputDevice.Volume = 1.0f;
                outputDevice.Play();
            }
        }

        public static readonly AudioPlayer INSTANCE = new();

        private readonly List<Audio> queue = new();

        private void PlayNext()
        {
            if (this.queue.Count == 0)
            {
                return;
            }

            Audio audio = this.queue[0];
            audio.OnFinished += (object sender, EventArgs e) =>
            {
                this.queue.Remove(audio);
                this.PlayNext();
            };

            audio.Play();
        }

        public void PlayAudio(string path)
        {
            Audio audio = new(path);

            if (Config.Instance().PlayAudioSequentially)
            {
                this.queue.Add(new Audio(path));
                if (this.queue.Count == 1)
                {
                    this.PlayNext();
                }
            }
            else
            {
                audio.Play();
            }
        }
    }
}

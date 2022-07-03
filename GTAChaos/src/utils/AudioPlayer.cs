// Copyright (c) 2019 Lordmau5
using GTAChaos.Effects;
using NAudio.Vorbis;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

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

            public bool IsExpired() => this.Expiry < DateTime.Now;

            public Task Play()
            {
                if (this.IsExpired())
                {
                    this.Finish();
                    return Task.CompletedTask;
                }

                Assembly a = Assembly.GetExecutingAssembly();
                Stream s = a.GetManifestResourceStream($"GTAChaos.assets.audio.{this.Path}.ogg");

                WaveStream stream = null;

                // ogg / Vorbis
                try
                {
                    stream = new VorbisWaveReader($"audio/{this.Path}.ogg");
                }
                catch { }

                // Iterate over supported formats
                if (stream == null)
                {
                    foreach (string format in this.supportedFormats)
                    {
                        try
                        {
                            stream = new MediaFoundationReader($"audio/{this.Path}.{format}");

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
                    this.Finish();
                    return Task.CompletedTask;
                }

                WaveOutEvent outputDevice = INSTANCE.GetWaveOutEvent();
                outputDevice.Init(stream);
                outputDevice.PlaybackStopped += (object sender, StoppedEventArgs e) => this.Finish();

                //outputDevice.Volume = Config.Instance().AudioVolume;
                outputDevice.Play();

                return Task.CompletedTask;
            }

            private void Finish() => OnFinished?.Invoke(this, new EventArgs());
        }

        public static readonly AudioPlayer INSTANCE = new();

        private readonly List<Audio> queue = new();

        private async Task PlayNext()
        {
            if (!Config.Instance().PlayAudioForEffects)
            {
                this.queue.Clear();
            }

            this.queue.RemoveAll(a => a.IsExpired());

            if (this.queue.Count == 0)
            {
                return;
            }

            Audio audio = this.queue[0];
            audio.OnFinished += (object sender, EventArgs e) =>
            {
                this.queue.Remove(audio);
                Task _ = this.PlayNext();
            };

            await Task.Run(async () => await audio.Play());
        }

        public async Task PlayAudio(string path, bool playNow = false)
        {
            Audio audio = new(path);

            if (!Config.Instance().PlayAudioSequentially || playNow)
            {
                await Task.Run(async () => await audio.Play());
            }
            else
            {
                this.queue.Add(new Audio(path));
                if (this.queue.Count == 1)
                {
                    await this.PlayNext();
                }
            }
        }

        private WaveOutEvent GetWaveOutEvent() => new();

        public void SetAudioVolume(float volume)
        {
            volume = Math.Max(0, Math.Min(volume, 1));

            this.GetWaveOutEvent().Volume = volume;
        }

        public float GetAudioVolume() => this.GetWaveOutEvent().Volume;

        public void CreateAndPrintAudioFileReadme()
        {
            try
            {
                string readmeDirectory = Path.Combine(Directory.GetCurrentDirectory(), "audio");
                Directory.CreateDirectory(readmeDirectory);

                string readmeFile = Path.Combine(readmeDirectory, "README.txt");

                using StreamWriter sw = new(readmeFile, false);
                sw.WriteLine("These are the available effects and their IDs that you can overwrite the sound clips for.");
                sw.WriteLine("The audio files can be in the following formats: .ogg, .mp3, .wav, .aac, .m4a");
                sw.WriteLine();
                sw.WriteLine("_____________________________________________________________________________");
                sw.WriteLine();

                foreach (WeightedRandomBag<AbstractEffect>.Entry entry in EffectDatabase.Effects.Get())
                {
                    AbstractEffect effect = entry.item;

                    sw.WriteLine($"{effect.GetDisplayName(DisplayNameType.UI)}");

                    if (effect.GetAudioVariations() > 0)
                    {
                        for (int i = 0; i < effect.GetAudioVariations(); i++)
                        {
                            sw.WriteLine($"{effect.GetID()}_{i}");
                        }
                    }
                    else
                    {
                        sw.WriteLine($"{effect.GetID()}");
                    }

                    sw.WriteLine();
                }

                sw.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}

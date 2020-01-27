// Copyright (c) 2019 Lordmau5
using NAudio.Wave;
using System;
using System.IO;
using System.Reflection;

namespace GtaChaos.Models.Utils
{
    public static class AudioPlayer
    {
        private static void PlayEmbeddedResource(string type, string path)
        {
            try
            {
                Assembly a = Assembly.GetExecutingAssembly();
                Stream s = a.GetManifestResourceStream($"GtaChaos.Models.{type}.{path}");

                MediaFoundationReader reader = null;
                try
                {
                    reader = new MediaFoundationReader($"{type}/{path}");
                }
                catch (Exception) // Didn't find an override audio file, try to use the embedded one
                {
                    reader = new StreamMediaFoundationReader(s);
                }

                WaveOutEvent outputDevice = new WaveOutEvent();
                outputDevice.Init(reader);

                outputDevice.Play();
            }
            catch (Exception)
            {
                // File not found >:(
            }
        }

        public static void PlayAudio(string res)
        {
            PlayEmbeddedResource("audio", $"{res}.m4a");
        }
    }
}

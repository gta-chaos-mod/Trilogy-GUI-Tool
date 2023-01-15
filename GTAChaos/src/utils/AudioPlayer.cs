// Copyright (c) 2019 Lordmau5
using GTAChaos.Effects;
using System;
using System.Diagnostics;
using System.IO;

namespace GTAChaos.Utils
{
    public class AudioPlayer
    {
        public static void CreateAndPrintAudioFileReadme()
        {
            try
            {
                string readmeFile = Path.Combine(Directory.GetCurrentDirectory(), "Audio_README.txt");

                using StreamWriter sw = new(readmeFile, false);
                sw.WriteLine("These are the available effects and their IDs that you can overwrite the sound clips for.");
                sw.WriteLine("The audio files can be in the following formats: .ogg, .mp3, .wav");
                sw.WriteLine("To overwrite a sound please put it in the \"GAME_DIRECTORY/ChaosMod/audio\" folder.");
                sw.WriteLine();
                sw.WriteLine("Example audio file name and path: GAME_DIRECTORY/ChaosMod/audio/effect_get_wasted.ogg");
                sw.WriteLine("_____________________________________________________________________________");
                sw.WriteLine();

                foreach (WeightedRandomBag<AbstractEffect>.Entry entry in EffectDatabase.Effects.Get())
                {
                    AbstractEffect effect = entry.item;

                    sw.WriteLine($"{effect.GetDisplayName(DisplayNameType.UI)}");

                    sw.WriteLine($"{effect.GetID()}");

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

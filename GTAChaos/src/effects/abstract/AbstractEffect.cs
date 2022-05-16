// Copyright (c) 2019 Lordmau5
using GTAChaos.Utils;
using System;
using System.Collections.Generic;

namespace GTAChaos.Effects
{
    public enum DisplayNameType
    {
        GAME,
        UI,
        STREAM
    }

    public abstract class AbstractEffect
    {
        public readonly Category Category;
        private readonly Dictionary<DisplayNameType, string> DisplayNames = new Dictionary<DisplayNameType, string>();
        public readonly string Word;
        public readonly int Duration;
        private float Multiplier;
        private string StreamVoter = "";
        private int rapidFire = 1;
        private bool streamEnabled = true;
        private string audioName = "";
        private int audioVariations = 0;

        public AbstractEffect(Category category, string displayName, string word, int duration = -1, float multiplier = 3.0f)
        {
            Category = category;
            SetDisplayNames(displayName);
            Word = word;
            Duration = duration;
            Multiplier = multiplier;

            category.AddEffectToCategory(this);
        }

        public abstract string GetId();

        private void SetDisplayNames(string displayName)
        {
            foreach (var nameType in Enum.GetValues(typeof(DisplayNameType)))
            {
                SetDisplayName((DisplayNameType) nameType, displayName);
            }
        }

        public AbstractEffect SetDisplayName(DisplayNameType type, string displayName)
        {
            DisplayNames[type] = displayName;
            return this;
        }

        public virtual string GetDisplayName(DisplayNameType type = DisplayNameType.GAME)
        {
            // We technically set all names already but let's safe-guard this anyway
            return DisplayNames.ContainsKey(type) ? DisplayNames[type] : DisplayNames[DisplayNameType.GAME];
        }

        public string GetVoter()
        {
            return StreamVoter;
        }

        public AbstractEffect SetTreamVoter(string voter)
        {
            StreamVoter = voter;
            return this;
        }

        public AbstractEffect ResetStreamVoter()
        {
            StreamVoter = "";
            return this;
        }

        public AbstractEffect DisableRapidFire()
        {
            rapidFire = 0;
            return this;
        }

        public AbstractEffect DisableTwitch()
        {
            streamEnabled = false;
            return this;
        }

        public AbstractEffect SetMultiplier(float multiplier)
        {
            Multiplier = multiplier;
            return this;
        }

        public float GetMultiplier()
        {
            return Multiplier;
        }

        public bool IsRapidFire()
        {
            return rapidFire == 1;
        }

        public bool IsTwitchEnabled()
        {
            return streamEnabled;
        }

        public AbstractEffect SetAudioFile(string name)
        {
            audioName = name;
            return this;
        }

        public AbstractEffect SetAudioVariations(int variations = 0)
        {
            audioVariations = variations;
            return this;
        }

        public virtual string GetAudioFile()
        {
            string file = string.IsNullOrEmpty(audioName) ? GetId() : audioName;

            if (audioVariations == 0)
            {
                return file;
            }
            else
            {
                Random random = new Random();
                return $"{file}_{random.Next(audioVariations)}";
            }
        }

        public virtual void RunEffect(int seed = -1, int _duration = -1)
        {
            if (Config.Instance().PlayAudioForEffects)
            {
                AudioPlayer.INSTANCE.PlayAudio(GetAudioFile());
            }
        }

        public int GetDuration(int duration = -1)
        {
            if (Duration != -1)
            {
                duration = Duration;
            }
            else
            {
                if (duration == -1)
                {
                    duration = Config.GetEffectDuration();
                }
                duration = (int)Math.Round(duration * Multiplier);
            }

            return duration;
        }

        public bool GetRapidFire()
        {
            bool rapidFire = false;
            if (Shared.IsStreamMode)
            {
                if (Shared.StreamVotingMode == 2)
                {
                    rapidFire = this.rapidFire == 1;
                }
            }

            return rapidFire;
        }
    }
}

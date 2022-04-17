// Copyright (c) 2019 Lordmau5
using GtaChaos.Models.Utils;
using System;

namespace GtaChaos.Models.Effects.@abstract
{
    public abstract class AbstractEffect
    {
        public readonly Category Category;
        private readonly string DisplayName;
        public readonly string Word;
        public readonly int Duration;
        private float Multiplier;
        private string TwitchVoter = "";
        private int rapidFire = 1;
        private bool twitchEnabled = true;
        private string audioName = "";
        private int audioVariations = 0;

        public AbstractEffect(Category category, string displayName, string word, int duration = -1, float multiplier = 1.0f)
        {
            Category = category;
            DisplayName = displayName;
            Word = word;
            Duration = duration;
            Multiplier = multiplier;

            category.AddEffectToCategory(this);
        }

        public abstract string GetId();

        public virtual string GetDisplayName(string displayName = "")
        {
            if (string.IsNullOrEmpty(displayName))
            {
                displayName = DisplayName;
            }

            return displayName;
        }

        public string GetVoter()
        {
            return TwitchVoter;
        }

        public AbstractEffect SetTwitchVoter(string voter)
        {
            TwitchVoter = voter;
            return this;
        }

        public AbstractEffect ResetTwitchVoter()
        {
            TwitchVoter = "";
            return this;
        }

        public AbstractEffect DisableRapidFire()
        {
            rapidFire = 0;
            return this;
        }

        public AbstractEffect DisableTwitch()
        {
            twitchEnabled = false;
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
            return twitchEnabled;
        }

        public AbstractEffect SetAudioFile(string name, int variations = 0)
        {
            audioName = name;
            audioVariations = variations;
            return this;
        }

        public virtual string GetAudioFile()
        {
            if (audioVariations == 0)
            {
                return audioName;
            }
            else
            {
                Random random = new Random();
                return $"{audioName}_{random.Next(audioVariations)}";
            }
        }

        public virtual void RunEffect(int seed = -1, int _duration = -1)
        {
            if (Config.Instance().PlayAudioForEffects)
            {
                AudioPlayer.PlayAudio(GetAudioFile());
            }
        }

        public int GetDuration(int duration = -1)
        {
            if (Duration > 0)
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
            if (Shared.IsTwitchMode)
            {
                if (Shared.TwitchVotingMode == 2)
                {
                    rapidFire = this.rapidFire == 1;
                }

                if (Config.Instance().Experimental_TwitchAnarchyMode)
                {
                    rapidFire = true;
                }
            }

            return rapidFire;
        }
    }
}

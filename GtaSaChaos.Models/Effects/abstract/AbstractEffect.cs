// Copyright (c) 2019 Lordmau5
using GtaChaos.Models.Utils;
using System;

namespace GtaChaos.Models.Effects.@abstract
{
    public abstract class AbstractEffect
    {
        public readonly string Id;
        public readonly Category Category;
        private readonly string Description;
        public readonly string Word;
        public readonly int Duration;
        private float Multiplier;
        private string Voter = "N/A";
        private int rapidFire = 1;
        private bool twitchEnabled = true;
        private string audioName = "";
        private int audioVariations = 0;

        public AbstractEffect(Category category, string description, string word, int duration = -1, float multiplier = 1.0f)
        {
            Id = category.AddEffectToCategory(this);
            Category = category;
            Description = description;
            Word = word;
            Duration = duration;
            Multiplier = multiplier;
        }

        public virtual string GetDescription()
        {
            return Description;
        }

        public string GetVoter()
        {
            return Voter;
        }

        public AbstractEffect SetVoter(string voter)
        {
            Voter = voter;
            return this;
        }

        public AbstractEffect ResetVoter()
        {
            Voter = "N/A";
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

        public void SendEffectToGame(string type, string function, int duration = -1, string description = "")
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

            if (string.IsNullOrEmpty(description))
            {
                description = GetDescription();
            }

            int rapidFire = 0;
            if (Shared.IsTwitchMode) {
                if (Shared.TwitchVotingMode == 2)
                {
                    rapidFire = this.rapidFire;
                }

                if (Config.Instance().Experimental_TwitchAnarchyMode)
                {
                    rapidFire = 1;
                }
            }

            ProcessHooker.SendEffectToGame(type, function, duration, description, Voter, rapidFire);
        }
    }
}

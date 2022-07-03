// Copyright (c) 2019 Lordmau5
using GTAChaos.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        private readonly Dictionary<DisplayNameType, string> DisplayNames = new();
        public readonly string Word;
        public readonly int Duration;
        private float Multiplier;
        private string Subtext = "";
        private int rapidFire = 1;
        private bool streamEnabled = true;
        private string audioName = "";
        private int audioVariations = 0;

        public AbstractEffect(Category category, string displayName, string word, int duration = -1, float multiplier = 3.0f)
        {
            this.Category = category;
            this.SetDisplayNames(displayName);
            this.Word = word;
            this.Duration = duration;
            this.Multiplier = multiplier;

            category.AddEffectToCategory(this);
        }

        public bool IsID(string id) => this.GetID().Equals(id) || this.GetID().Equals($"effect_{id}");

        public abstract string GetID();

        protected void SetDisplayNames(string displayName)
        {
            foreach (object nameType in Enum.GetValues(typeof(DisplayNameType)))
            {
                this.SetDisplayName((DisplayNameType)nameType, displayName);
            }
        }

        public AbstractEffect SetDisplayName(DisplayNameType type, string displayName)
        {
            this.DisplayNames[type] = displayName;
            return this;
        }

        public virtual string GetDisplayName(DisplayNameType type = DisplayNameType.GAME) =>
            // We technically set all names already but let's safe-guard this anyway
            this.DisplayNames.ContainsKey(type) ? this.DisplayNames[type] : this.DisplayNames[DisplayNameType.GAME];

        public string GetSubtext() => this.Subtext;

        public AbstractEffect SetSubtext(string subtext)
        {
            this.Subtext = subtext;
            return this;
        }

        public AbstractEffect ResetSubtext()
        {
            this.Subtext = "";
            return this;
        }

        public AbstractEffect DisableRapidFire()
        {
            this.rapidFire = 0;
            return this;
        }

        public AbstractEffect DisableStream()
        {
            this.streamEnabled = false;
            return this;
        }

        public AbstractEffect SetMultiplier(float multiplier)
        {
            this.Multiplier = multiplier;
            return this;
        }

        public float GetMultiplier() => this.Multiplier;

        public bool IsRapidFire() => this.rapidFire == 1;

        public bool IsTwitchEnabled() => this.streamEnabled;

        public AbstractEffect SetAudioFile(string name)
        {
            this.audioName = name;
            return this;
        }

        public int GetAudioVariations() => this.audioVariations;

        public AbstractEffect SetAudioVariations(int variations = 0)
        {
            this.audioVariations = variations;
            return this;
        }

        public virtual string GetAudioFile()
        {
            string file = string.IsNullOrEmpty(this.audioName) ? this.GetID() : this.audioName;

            if (this.audioVariations == 0)
            {
                return file;
            }
            else
            {
                Random random = new();
                return $"{file}_{random.Next(this.audioVariations)}";
            }
        }

        public virtual async Task RunEffect(int seed = -1, int _duration = -1)
        {
            if (Config.Instance().PlayAudioForEffects && Shared.StreamVotingMode != Shared.VOTING_MODE.RAPID_FIRE)
            {
                await AudioPlayer.INSTANCE.PlayAudio(this.GetAudioFile());
            }
        }

        public int GetDuration(int duration = -1)
        {
            if (duration != -1)
            {
                return duration;
            }

            if (this.Duration != -1)
            {
                duration = Math.Min(Config.GetEffectDuration(), this.Duration);
            }
            else
            {
                if (duration == -1)
                {
                    duration = Config.GetEffectDuration();
                }

                duration = (int)Math.Round(duration * this.Multiplier);
            }

            return duration;
        }

        public bool GetRapidFire()
        {
            bool rapidFire = false;
            if (Shared.IsStreamMode)
            {
                if (Shared.StreamVotingMode == Shared.VOTING_MODE.RAPID_FIRE)
                {
                    rapidFire = this.rapidFire == 1;
                }
            }

            return rapidFire;
        }
    }
}

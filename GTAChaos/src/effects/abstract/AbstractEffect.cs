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

        public virtual Task RunEffect(int seed = -1, int _duration = -1) => Task.CompletedTask;

        public virtual bool IsCooldownable() => true;

        public int GetDuration(int duration = -1)
        {
            if (duration != -1)
            {
                return duration;
            }

            if (this.Duration != -1)
            {
                duration = this.Duration;
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

        public bool GetRapidFire(bool overrideCheck = false)
        {
            if (overrideCheck)
            {
                return this.rapidFire == 1;
            }

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

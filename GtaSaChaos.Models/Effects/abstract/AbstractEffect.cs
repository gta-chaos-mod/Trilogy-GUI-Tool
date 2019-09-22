// Copyright (c) 2019 Lordmau5

using GtaSaChaos.Models.Utils;

namespace GtaSaChaos.Models.Effects.@abstract
{
    public abstract class AbstractEffect
    {
        public readonly string Id;
        public readonly Category Category;
        private readonly string Description;
        public readonly string Word;
        private string Voter = "N/A";
        private int rapidFire = 1;
        private bool twitchEnabled = true;

        public AbstractEffect(Category category, string description, string word)
        {
            Id = category.AddEffectToCategory(this);
            Category = category;
            Description = description;
            Word = word;
        }

        public virtual string GetDescription()
        {
            return Description;
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

        public bool IsRapidFire()
        {
            return rapidFire == 1;
        }

        public bool IsTwitchEnabled()
        {
            return twitchEnabled;
        }

        public abstract void RunEffect();

        public void SendEffectToGame(string type, string function, int duration = -1, string description = "", int multiplier = 1)
        {
            if (duration == -1)
            {
                duration = Config.GetEffectDuration();
            }

            duration *= multiplier;

            if (string.IsNullOrEmpty(description))
            {
                description = GetDescription();
            }

            ProcessHooker.SendEffectToGame(type, function, duration, description, Voter, Config.Instance.TwitchVotingMode == 2 ? rapidFire : 0);
        }
    }
}

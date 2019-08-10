using GTA_SA_Chaos.util;
using System;

namespace GTA_SA_Chaos.effects
{
    public abstract class AbstractEffect
    {
        public readonly string Id;
        public readonly Category Category;
        private readonly string Description;
        public readonly string Word;

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

            ProcessHooker.SendEffectToGame(type, function, duration, description);
        }
    }
}

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

        public void StoreEffectToFile()
        {
            Config.DoStoreLastEffectToFile(this);

            ProcessHooker.NewThreadStartClient("text");
            ProcessHooker.NewThreadStartClient(GetDescription());
        }
    }
}

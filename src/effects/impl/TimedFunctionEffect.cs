using GTA_SA_Chaos.util;
using System;

namespace GTA_SA_Chaos.effects
{
    public class TimedFunctionEffect : AbstractEffect
    {
        private readonly string type;
        private readonly string func;
        private readonly int duration;

        public TimedFunctionEffect(Category category, string description, string word, string _type, string _func, int _duration = -1)
            : base(category, description, word)
        {
            type = _type;
            func = _func;
            duration = _duration;
        }

        public override void RunEffect()
        {
            StoreEffectToFile();

            int dur = duration;
            if (dur == -1)
            {
                dur = Config.GetEffectDuration();
            }

            ProcessHooker.NewThreadStartClient(type);
            ProcessHooker.NewThreadStartClient($"{func}:{dur}:{GetDescription()}");
        }
    }
}

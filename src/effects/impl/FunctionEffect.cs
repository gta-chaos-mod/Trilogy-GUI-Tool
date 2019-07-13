using GTA_SA_Chaos.util;
using System;

namespace GTA_SA_Chaos.effects
{
    public class FunctionEffect : AbstractEffect
    {
        private readonly string type;
        private readonly string func;

        public FunctionEffect(Category category, string description, string word, string _type, string _func)
            : base(category, description, word)
        {
            type = _type;
            func = _func;
        }

        public override void RunEffect()
        {
            StoreEffectToFile();

            ProcessHooker.SendPipeMessage(type);
            ProcessHooker.SendPipeMessage(func);
        }
    }
}

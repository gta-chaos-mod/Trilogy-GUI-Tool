using GTA_SA_Chaos.util;
using System;

namespace GTA_SA_Chaos.effects
{
    public class FunctionEffect : AbstractEffect
    {
        private readonly string type;
        private readonly string function;
        private readonly int duration;

        public FunctionEffect(Category category, string description, string word, string _type, string _function, int _duration = -1)
            : base(category, description, word)
        {
            type = _type;
            function = _function;
            duration = _duration;
        }

        public override void RunEffect()
        {
            SendEffectToGame("set_seed", RandomHandler.Next(9999999).ToString());
            SendEffectToGame(type, function, duration);
        }
    }
}

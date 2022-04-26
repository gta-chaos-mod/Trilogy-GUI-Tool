// Copyright (c) 2019 Lordmau5
using GtaChaos.Models.Effects.@abstract;
using GtaChaos.Models.Utils;

namespace GtaChaos.Models.Effects.impl
{
    public class FunctionEffect : AbstractEffect
    {
        private readonly string EffectID;

        public FunctionEffect(Category category, string displayName, string word, string effectID, int duration = -1, float multiplier = 3.0f)
            : base(category, displayName, word, duration, multiplier)
        {
            EffectID = $"effect_{effectID}";
        }

        public override string GetId()
        {
            return EffectID;
        }

        public override string GetAudioFile()
        {
            string file = base.GetAudioFile();

            return string.IsNullOrWhiteSpace(file) ? EffectID : file;
        }

        public override void RunEffect(int seed = -1, int duration = -1)
        {
            base.RunEffect(seed, duration);

            seed = seed == -1 ? RandomHandler.Next(9999999) : seed;

            WebsocketHandler.INSTANCE.SendEffectToGame(EffectID, new
            {
                seed
            }, GetDuration(duration), GetDisplayName(), GetVoter(), GetRapidFire());
        }
    }
}

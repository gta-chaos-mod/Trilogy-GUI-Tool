// Copyright (c) 2019 Lordmau5
using GTAChaos.Utils;
using System.Threading.Tasks;

namespace GTAChaos.Effects
{
    public class FunctionEffect : AbstractEffect
    {
        private readonly string EffectID;

        public FunctionEffect(Category category, string displayName, string word, string effectID, int duration = -1, float multiplier = 3.0f)
            : base(category, displayName, word, duration, multiplier)
        {
            if (effectID.StartsWith("effect_"))
            {
                throw new System.Exception($"Effect '{displayName}' has the 'effect_' prefix!");
            }

            this.EffectID = $"effect_{effectID}";
        }

        public override string GetID() => this.EffectID;

        public override async Task RunEffect(int seed = -1, int duration = -1)
        {
            await base.RunEffect(seed, duration);

            seed = seed == -1 ? RandomHandler.Next(9999999) : seed;

            WebsocketHandler.INSTANCE.SendEffectToGame(this.EffectID, new
            {
                seed
            }, this.GetDuration(duration), this.GetDisplayName(), this.GetSubtext(), this.GetRapidFire());
        }
    }
}

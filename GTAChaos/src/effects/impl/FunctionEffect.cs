// Copyright (c) 2019 Lordmau5
using GTAChaos.Utils;

namespace GTAChaos.Effects
{
    public class FunctionEffect : AbstractEffect
    {
        private readonly string EffectID;

        public FunctionEffect(Category category, string displayName, string word, string effectID, int duration = -1, float multiplier = 3.0f)
            : base(category, displayName, word, duration, multiplier) => this.EffectID = $"effect_{effectID}";

        public override string GetID() => this.EffectID;

        public override string GetAudioFile()
        {
            string file = base.GetAudioFile();

            return string.IsNullOrWhiteSpace(file) ? this.EffectID : file;
        }

        public override void RunEffect(int seed = -1, int duration = -1)
        {
            base.RunEffect(seed, duration);

            seed = seed == -1 ? RandomHandler.Next(9999999) : seed;

            WebsocketHandler.INSTANCE.SendEffectToGame(this.EffectID, new
            {
                seed
            }, this.GetDuration(duration), this.GetDisplayName(), this.GetVoter(), this.GetRapidFire());
        }
    }
}

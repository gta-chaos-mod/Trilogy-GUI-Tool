// Copyright (c) 2019 Lordmau5
using GTAChaos.Utils;

namespace GTAChaos.Effects
{
    public class FakeCrashEffect : AbstractEffect
    {
        private readonly string EffectID = "effect_fake_crash";

        public FakeCrashEffect(string description, string word)
            : base(Category.CustomEffects, description, word) => this.DisableRapidFire();

        public override string GetID() => this.EffectID;

        public override void RunEffect(int seed = -1, int duration = -1)
        {
            base.RunEffect(seed, duration);

            WebsocketHandler.INSTANCE.SendEffectToGame(this.EffectID, new
            {
                realEffectName = "Fake Crash"
            }, this.GetDuration(duration), this.GetDisplayName(), this.GetSubtext(), this.GetRapidFire());
        }
    }
}

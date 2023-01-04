// Copyright (c) 2019 Lordmau5
using GTAChaos.Utils;
using System.Threading.Tasks;

namespace GTAChaos.Effects
{
    public class FakeCrashEffect : AbstractEffect
    {
        private readonly string EffectID = "effect_fake_crash";

        public FakeCrashEffect(string description, string word)
            : base(Category.CustomEffects_Generic, description, word)
        {
            this.SetDisplayName(DisplayNameType.UI, "Fake Crash");
            this.DisableRapidFire();
        }

        public override string GetID() => this.EffectID;

        public override async Task RunEffect(int seed = -1, int duration = -1)
        {
            await base.RunEffect(seed, duration);

            WebsocketHandler.INSTANCE.SendEffectToGame(this.EffectID, new
            {
                realEffectName = "Fake Crash"
            }, this.GetDuration(duration), this.GetDisplayName(), this.GetSubtext(), this.GetRapidFire());
        }
    }
}

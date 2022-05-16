// Copyright (c) 2019 Lordmau5
using GTAChaos.Utils;

namespace GTAChaos.Effects
{
    public class FakeTeleportEffect : AbstractEffect
    {
        private readonly string EffectID = "effect_fake_teleport";

        public FakeTeleportEffect(string description, string word, int duration)
            : base(Category.Teleportation, description, word, duration) => this.DisableRapidFire();

        public override string GetId() => this.EffectID;

        public override void RunEffect(int seed = -1, int duration = -1)
        {
            base.RunEffect(seed, duration);

            WebsocketHandler.INSTANCE.SendEffectToGame(this.EffectID, new
            {
                realEffectName = "Fake Teleport"
            }, this.GetDuration(duration), "TP To A Tower", this.GetVoter(), this.GetRapidFire());
        }
    }
}

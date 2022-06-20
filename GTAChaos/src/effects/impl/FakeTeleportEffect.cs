// Copyright (c) 2019 Lordmau5
using GTAChaos.Utils;

namespace GTAChaos.Effects
{
    public class FakeTeleportEffect : AbstractEffect
    {
        private readonly string EffectID = "effect_fake_teleport";

        public FakeTeleportEffect(string description, string word)
            : base(Category.Teleportation, description, word) => this.DisableRapidFire();

        public override string GetID() => this.EffectID;

        public override void RunEffect(int seed = -1, int duration = -1)
        {
            base.RunEffect(seed, duration);

            Location location = Location.Locations[RandomHandler.Next(Location.Locations.Count)];

            WebsocketHandler.INSTANCE.SendEffectToGame(this.EffectID, new
            {
                realEffectName = "Fake Teleport",
                posX = location.X,
                posY = location.Y,
                posZ = location.Z
            }, this.GetDuration(duration), location.GetDisplayName(), this.GetSubtext(), this.GetRapidFire());
        }
    }
}

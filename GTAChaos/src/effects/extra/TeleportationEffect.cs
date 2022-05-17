// Copyright (c) 2019 Lordmau5
using GTAChaos.Utils;

namespace GTAChaos.Effects
{
    public class TeleportationEffect : AbstractEffect
    {
        private readonly Location location;

        public TeleportationEffect(string description, string word, Location _location)
            : base(Category.Teleportation, description, word)
        {
            this.location = _location;

            this.DisableRapidFire();
        }

        public override string GetID() => $"teleport_{this.location.Id}";

        public override void RunEffect(int seed = -1, int duration = -1)
        {
            base.RunEffect(seed, duration);

            WebsocketHandler.INSTANCE.SendEffectToGame("effect_teleport", new
            {
                posX = this.location.X,
                posY = this.location.Y,
                posZ = this.location.Z
            }, this.GetDuration(duration), this.GetDisplayName(), this.GetVoter(), this.GetRapidFire());
        }
    }
}

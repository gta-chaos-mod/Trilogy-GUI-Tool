// Copyright (c) 2019 Lordmau5
using GtaChaos.Models.Effects.@abstract;
using GtaChaos.Models.Utils;

namespace GtaChaos.Models.Effects.extra
{
    public class TeleportationEffect : AbstractEffect
    {
        private readonly Location location;

        public TeleportationEffect(string description, string word, Location _location)
            : base(Category.Teleportation, description, word)
        {
            location = _location;

            DisableRapidFire();
        }

        public override string GetId()
        {
            return $"teleport_{location.Id}";
        }

        public override void RunEffect(int seed = -1, int duration = -1)
        {
            base.RunEffect(seed, duration);

            WebsocketHandler.INSTANCE.SendEffectToGame("effect_teleport", new
            {
                posX = location.X,
                posY = location.Y,
                posZ = location.Z
            }, GetDuration(duration), GetDisplayName(), GetVoter(), GetRapidFire());
        }
    }
}

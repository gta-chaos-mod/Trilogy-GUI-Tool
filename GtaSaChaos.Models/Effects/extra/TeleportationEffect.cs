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

        public override void RunEffect()
        {
            SendEffectToGame("teleport", location.ToString());
        }
    }
}

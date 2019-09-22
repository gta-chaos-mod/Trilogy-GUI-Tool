// Copyright (c) 2019 Lordmau5

using GtaSaChaos.Models.Effects.@abstract;
using GtaSaChaos.Models.Utils;

namespace GtaSaChaos.Models.Effects.extra
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

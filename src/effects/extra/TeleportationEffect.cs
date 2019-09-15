// Copyright (c) 2019 Lordmau5
using GTA_SA_Chaos.util;

namespace GTA_SA_Chaos.effects
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

// Copyright (c) 2019 Lordmau5
using GTAChaos.Utils;
using System;

namespace GTAChaos.Effects
{
    internal class SpawnVehicleEffect : AbstractEffect
    {
        private readonly int VehicleID;

        public SpawnVehicleEffect(string word, int vehicleID)
            : base(Category.Spawning, "Spawn Vehicle", word)
        {
            VehicleID = vehicleID;

            if (VehicleID == -1)
            {
                SetDisplayName(DisplayNameType.UI, "Spawn Random Vehicle");
            }
            else
            {
                VehicleID = Math.Max(400, Math.Min(vehicleID, 611));
                SetDisplayName(DisplayNameType.UI, $"Spawn {VehicleNames.GetVehicleName(vehicleID)}");
            }
        }

        public override string GetId()
        {
            return $"spawn_vehicle_{VehicleID}";
        }

        public override void RunEffect(int seed = -1, int duration = -1)
        {
            base.RunEffect(seed, duration);

            int vehicleID = VehicleID;
            if (vehicleID == -1)
            {
                vehicleID = RandomHandler.Next(400, 611);
            }

            string spawnString = $"Spawn {VehicleNames.GetVehicleName(vehicleID)}";

            WebsocketHandler.INSTANCE.SendEffectToGame("effect_spawn_vehicle", new
            {
                vehicleID
            }, GetDuration(duration), spawnString, GetVoter(), GetRapidFire());
        }
    }
}

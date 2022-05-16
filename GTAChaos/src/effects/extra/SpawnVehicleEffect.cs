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
            this.VehicleID = vehicleID;

            if (this.VehicleID == -1)
            {
                this.SetDisplayName(DisplayNameType.UI, "Spawn Random Vehicle");
            }
            else
            {
                this.VehicleID = Math.Max(400, Math.Min(vehicleID, 611));
                this.SetDisplayName(DisplayNameType.UI, $"Spawn {VehicleNames.GetVehicleName(vehicleID)}");
            }
        }

        public override string GetId() => $"spawn_vehicle_{this.VehicleID}";

        public override void RunEffect(int seed = -1, int duration = -1)
        {
            base.RunEffect(seed, duration);

            int vehicleID = this.VehicleID;
            if (vehicleID == -1)
            {
                vehicleID = RandomHandler.Next(400, 611);
            }

            string spawnString = $"Spawn {VehicleNames.GetVehicleName(vehicleID)}";

            WebsocketHandler.INSTANCE.SendEffectToGame("effect_spawn_vehicle", new
            {
                vehicleID
            }, this.GetDuration(duration), spawnString, this.GetVoter(), this.GetRapidFire());
        }
    }
}

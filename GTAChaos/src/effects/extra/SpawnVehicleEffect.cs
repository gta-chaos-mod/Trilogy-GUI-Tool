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
                this.SetDisplayNames("Spawn Random Vehicle");
            }
            else
            {
                this.VehicleID = Math.Max(400, Math.Min(vehicleID, 611));
                this.SetDisplayNames($"Spawn {VehicleNames.GetVehicleName(vehicleID)}");
            }
        }

        public override string GetID() => $"spawn_vehicle_{this.VehicleID}";

        public override void RunEffect(int seed = -1, int duration = -1)
        {
            base.RunEffect(seed, duration);

            int vehicleID = this.VehicleID;
            if (vehicleID == -1)
            {
                vehicleID = RandomHandler.Next(400, 611);
            }

            WebsocketHandler.INSTANCE.SendEffectToGame("effect_spawn_vehicle", new
            {
                vehicleID
            }, this.GetDuration(duration), this.GetDisplayName(), this.GetVoter(), this.GetRapidFire());
        }
    }
}

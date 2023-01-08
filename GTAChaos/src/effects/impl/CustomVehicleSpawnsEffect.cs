// Copyright (c) 2019 Lordmau5
using GTAChaos.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GTAChaos.Effects
{
    public class CustomVehicleSpawnsEffect : AbstractEffect
    {
        private readonly string EffectID = "effect_custom_vehicle_spawns";
        private readonly int VehicleID;

        public CustomVehicleSpawnsEffect(int vehicleID, string word)
            : base(Category.CustomEffects_Traffic, "Traffic Is Vehicle", word)
        {

            this.VehicleID = vehicleID;

            if (this.VehicleID == -1)
            {
                this.SetDisplayNames("Traffic Is Random Vehicle");
            }
            else
            {
                this.VehicleID = Math.Max(400, Math.Min(vehicleID, 611));
                this.SetDisplayNames($"Traffic Is {VehicleDatabase.GetVehicleName(vehicleID)}");
            }
        }

        public override string GetID() => $"custom_vehicle_spawns_{this.VehicleID}";

        public override async Task RunEffect(int seed = -1, int duration = -1)
        {
            await base.RunEffect(seed, duration);

            string gameDisplayName = this.GetDisplayName();

            int vehicleID = this.VehicleID;
            if (vehicleID == -1)
            {
                List<int> potentialVehicles = VehicleDatabase.GetPotentialVehicles();

                int randomVehicle = RandomHandler.Next(0, potentialVehicles.Count - 1);
                vehicleID = potentialVehicles[randomVehicle];
                gameDisplayName = $"Traffic Is {VehicleDatabase.GetVehicleName(vehicleID)}";
            }

            WebsocketHandler.INSTANCE.SendEffectToGame(this.EffectID, new
            {
                vehicleID,
            }, this.GetDuration(duration), gameDisplayName, this.GetSubtext(), this.GetRapidFire());
        }
    }
}

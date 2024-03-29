﻿// Copyright (c) 2019 Lordmau5
using GTAChaos.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GTAChaos.Effects
{
    internal class SpawnVehicleEffect : AbstractEffect
    {
        private readonly string EffectID = "effect_spawn_vehicle";
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
                this.SetDisplayNames($"Spawn {VehicleDatabase.GetVehicleName(vehicleID)}");
            }
        }

        public override string GetID() => $"spawn_vehicle_{this.VehicleID}";

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
                gameDisplayName = $"Spawn {VehicleDatabase.GetVehicleName(vehicleID)}";
            }

            WebsocketHandler.INSTANCE.SendEffectToGame(this.EffectID, new
            {
                vehicleID
            }, this.GetDuration(duration), gameDisplayName, this.GetSubtext(), this.GetRapidFire());
        }
    }
}

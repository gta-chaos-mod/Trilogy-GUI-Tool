// Copyright (c) 2019 Lordmau5
using GTAChaos.Effects;
using System.Collections.Generic;

namespace GTAChaos.Utils
{
    public sealed class Category
    {
        public readonly string Name;
        public readonly List<AbstractEffect> Effects;

        public static readonly List<Category> Categories = new();

        private Category(string name)
        {
            this.Name = name;
            this.Effects = new List<AbstractEffect>();

            if (!Categories.Contains(this))
            {
                Categories.Add(this);

                Categories.Sort((first, second) => string.Compare(first.Name, second.Name, System.StringComparison.CurrentCultureIgnoreCase));
            }
        }

        public void AddEffectToCategory(AbstractEffect effect) => this.Effects.Add(effect);

        public int GetEffectCount() => this.Effects.Count;

        public void ClearEffects() => this.Effects.Clear();

        public static readonly Category WeaponsAndHealth = new("Weapons & Health");
        public static readonly Category WantedLevel = new("Wanted Level");
        public static readonly Category Weather = new("Weather");
        public static readonly Category Spawning = new("Spawning");
        public static readonly Category Time = new("Time");
        public static readonly Category VehiclesTraffic = new("Vehicles & Traffic");
        public static readonly Category NPCs = new("NPCs");
        public static readonly Category PlayerModifications = new("Player Modifications");
        public static readonly Category Stats = new("Stats");
        public static readonly Category Teleportation = new("Teleportation");

        public static readonly Category CustomEffects_Audio = new("Custom Effects - Audio");
        public static readonly Category CustomEffects_Framerate = new("Custom Effects - Framerate");
        public static readonly Category CustomEffects_Generic = new("Custom Effects - Generic");
        public static readonly Category CustomEffects_Gravity = new("Custom Effects - Gravity");
        public static readonly Category CustomEffects_HUD = new("Custom Effects - HUD");
        public static readonly Category CustomEffects_Mission = new("Custom Effects - Mission");
        public static readonly Category CustomEffects_NPCs = new("Custom Effects - NPCs");
        public static readonly Category CustomEffects_Objects = new("Custom Effects - Objects");
        public static readonly Category CustomEffects_Ped = new("Custom Effects - Ped");
        public static readonly Category CustomEffects_Player = new("Custom Effects - Player");
        public static readonly Category CustomEffects_Stats = new("Custom Effects - Stats");
        public static readonly Category CustomEffects_Traffic = new("Custom Effects - Traffic");
        public static readonly Category CustomEffects_Vehicle = new("Custom Effects - Vehicle");
        public static readonly Category CustomEffects_Wanted = new("Custom Effects - Wanted");
        public static readonly Category CustomEffects_Weather = new("Custom Effects - Weather");

    }
}

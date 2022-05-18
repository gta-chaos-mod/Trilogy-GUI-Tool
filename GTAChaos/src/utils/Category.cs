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
        public static readonly Category CustomEffects = new("Custom Effects");
        public static readonly Category Teleportation = new("Teleportation");
    }
}

// Copyright (c) 2019 Lordmau5
using GTAChaos.Effects;
using System.Collections.Generic;

namespace GTAChaos.Utils
{
    public sealed class Category
    {
        public readonly string Name;
        public readonly List<AbstractEffect> Effects;

        public static readonly List<Category> Categories = new List<Category>();

        private Category(string name)
        {
            Name = name;
            Effects = new List<AbstractEffect>();

            if (!Categories.Contains(this))
            {
                Categories.Add(this);

                Categories.Sort((first, second) => string.Compare(first.Name, second.Name, System.StringComparison.CurrentCultureIgnoreCase));
            }
        }

        public void AddEffectToCategory(AbstractEffect effect)
        {
            Effects.Add(effect);
        }

        public int GetEffectCount()
        {
            return Effects.Count;
        }

        public void ClearEffects()
        {
            Effects.Clear();
        }

        public static readonly Category WeaponsAndHealth = new Category("Weapons & Health");
        public static readonly Category WantedLevel = new Category("Wanted Level");
        public static readonly Category Weather = new Category("Weather");
        public static readonly Category Spawning = new Category("Spawning");
        public static readonly Category Time = new Category("Time");
        public static readonly Category VehiclesTraffic = new Category("Vehicles & Traffic");
        public static readonly Category PedsAndCo = new Category("Peds & Co.");
        public static readonly Category PlayerModifications = new Category("Player Modifications");
        public static readonly Category Stats = new Category("Stats");
        public static readonly Category CustomEffects = new Category("Custom Effects");
        public static readonly Category Teleportation = new Category("Teleportation");
    }
}

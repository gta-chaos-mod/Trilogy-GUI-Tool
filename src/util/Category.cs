using GTA_SA_Chaos.effects;
using System.Collections.Generic;

namespace GTA_SA_Chaos.util
{
    public sealed class Category
    {
        public readonly string Name;
        public readonly string Prefix;
        public readonly List<AbstractEffect> Effects;

        private Category(string name, string prefix)
        {
            Name = name;
            Prefix = prefix;
            Effects = new List<AbstractEffect>();
        }

        public string AddEffectToCategory(AbstractEffect effect)
        {
            Effects.Add(effect);
            return Prefix + Effects.Count;
        }

        public static readonly Category WeaponsAndHealth = new Category("Weapons & Health", "HE");
        public static readonly Category WantedLevel = new Category("Wanted Level", "WA");
        public static readonly Category Weather = new Category("Weather", "WE");
        public static readonly Category Spawning = new Category("Spawning", "SP");
        public static readonly Category Time = new Category("Time", "TI");
        public static readonly Category VehiclesTraffic = new Category("Vehicles & Traffic", "VE");
        public static readonly Category PedsAndCo = new Category("Peds & Co.", "PE");
        public static readonly Category PlayerModifications = new Category("Player Modifications", "MO");
        public static readonly Category Stats = new Category("Stats", "ST");
        public static readonly Category CustomEffects = new Category("Custom Effects", "CE");
        public static readonly Category Teleportation = new Category("Teleportation", "TP");
    }
}

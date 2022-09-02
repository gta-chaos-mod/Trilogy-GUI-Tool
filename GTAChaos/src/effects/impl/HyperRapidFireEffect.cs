// Copyright (c) 2019 Lordmau5

namespace GTAChaos.Effects
{
    public class HyperRapidFireEffect : RapidFireEffect
    {
        public HyperRapidFireEffect(string description, string word, string id, int duration = -1, float multiplier = 3)
            : base(description, word, id, duration, multiplier)
        {
            this.effects = 20;
            this.delay = 500;
        }
    }
}

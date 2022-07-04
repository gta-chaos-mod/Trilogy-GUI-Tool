// Copyright (c) 2019 Lordmau5

namespace GTAChaos.Effects
{
    public class DiscountRapidFireEffect : RapidFireEffect
    {
        public DiscountRapidFireEffect(string description, string word, string id, int duration = -1, float multiplier = 3)
            : base(description, word, id, duration, multiplier)
        {
            this.effects = 1;
            this.delay = 1250;
        }
    }
}

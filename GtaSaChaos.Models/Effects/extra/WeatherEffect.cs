// Copyright (c) 2019 Lordmau5

using GtaSaChaos.Models.Effects.@abstract;
using GtaSaChaos.Models.Utils;

namespace GtaSaChaos.Models.Effects.extra
{
    public class WeatherEffect : AbstractEffect
    {
        private readonly int weatherID;

        public WeatherEffect(string description, string word, int _weatherID)
            : base(Category.Weather, description, word)
        {
            weatherID = _weatherID;
        }

        public override void RunEffect()
        {
            SendEffectToGame("weather", weatherID.ToString());
        }
    }
}

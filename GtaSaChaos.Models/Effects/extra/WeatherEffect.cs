// Copyright (c) 2019 Lordmau5
using GtaChaos.Models.Effects.@abstract;
using GtaChaos.Models.Utils;

namespace GtaChaos.Models.Effects.extra
{
    public class WeatherEffect : AbstractEffect
    {
        private readonly int weatherID;

        public WeatherEffect(string description, string word, int _weatherID)
            : base(Category.Weather, description, word)
        {
            weatherID = _weatherID;
        }

        public override void RunEffect(int seed = -1, int _duration = -1)
        {
            SendEffectToGame("weather", weatherID.ToString(), (_duration == -1 ? -1 : _duration));
        }
    }
}

// Copyright (c) 2019 Lordmau5
using GtaChaos.Models.Effects.@abstract;
using GtaChaos.Models.Utils;

namespace GtaChaos.Models.Effects.extra
{
    public class WeatherEffect : AbstractEffect
    {
        private readonly int weatherID;

        public WeatherEffect(string description, string word, int _weatherID, int duration = -1)
            : base(Category.Weather, description, word, duration)
        {
            weatherID = _weatherID;
        }

        public override string GetId()
        {
            return $"weather_{weatherID}";
        }

        public override void RunEffect(int seed = -1, int duration = -1)
        {
            base.RunEffect(seed, duration);

            ProcessHooker.SendEffectToGame("effect_weather", new
            {
                weatherID
            }, GetDuration(duration), GetDisplayName(), GetVoter(), GetRapidFire());
        }
    }
}

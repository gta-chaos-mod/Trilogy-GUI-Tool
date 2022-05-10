// Copyright (c) 2019 Lordmau5
using GTAChaos.Effects;
using GTAChaos.Utils;

namespace GTAChaos.Effects.extra
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

            WebsocketHandler.INSTANCE.SendEffectToGame("effect_weather", new
            {
                weatherID
            }, GetDuration(duration), GetDisplayName(), GetVoter(), GetRapidFire());
        }
    }
}

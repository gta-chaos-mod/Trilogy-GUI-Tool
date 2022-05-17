// Copyright (c) 2019 Lordmau5
using GTAChaos.Utils;

namespace GTAChaos.Effects
{
    public class WeatherEffect : AbstractEffect
    {
        private readonly int weatherID;

        public WeatherEffect(string description, string word, int _weatherID, int duration = -1)
            : base(Category.Weather, description, word, duration) => this.weatherID = _weatherID;

        public override string GetID() => $"weather_{this.weatherID}";

        public override void RunEffect(int seed = -1, int duration = -1)
        {
            base.RunEffect(seed, duration);

            WebsocketHandler.INSTANCE.SendEffectToGame("effect_weather", new
            {
                this.weatherID
            }, this.GetDuration(duration), this.GetDisplayName(), this.GetVoter(), this.GetRapidFire());
        }
    }
}

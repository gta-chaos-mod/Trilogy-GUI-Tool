// Copyright (c) 2019 Lordmau5
using GTAChaos.Utils;
using System.Threading.Tasks;

namespace GTAChaos.Effects
{
    public class WeatherEffect : AbstractEffect
    {
        private readonly string EffectID = "effect_weather";
        private readonly int WeatherID;

        public WeatherEffect(string description, string word, int _weatherID, int duration = -1)
            : base(Category.Weather, description, word, duration) => this.WeatherID = _weatherID;

        public override string GetID() => $"weather_{this.WeatherID}";

        public override async Task RunEffect(int seed = -1, int duration = -1)
        {
            await base.RunEffect(seed, duration);

            WebsocketHandler.INSTANCE.SendEffectToGame(this.EffectID, new
            {
                weatherID = this.WeatherID
            }, this.GetDuration(duration), this.GetDisplayName(), this.GetSubtext(), this.GetRapidFire());
        }
    }
}

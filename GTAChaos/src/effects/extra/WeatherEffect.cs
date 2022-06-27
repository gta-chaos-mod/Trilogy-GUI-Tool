// Copyright (c) 2019 Lordmau5
using GTAChaos.Utils;
using System.Threading.Tasks;

namespace GTAChaos.Effects
{
    public class WeatherEffect : AbstractEffect
    {
        private readonly int weatherID;

        public WeatherEffect(string description, string word, int _weatherID, int duration = -1)
            : base(Category.Weather, description, word, duration) => this.weatherID = _weatherID;

        public override string GetID() => $"weather_{this.weatherID}";

        public override async Task RunEffect(int seed = -1, int duration = -1)
        {
            await base.RunEffect(seed, duration);

            WebsocketHandler.INSTANCE.SendEffectToGame("effect_weather", new
            {
                this.weatherID
            }, this.GetDuration(duration), this.GetDisplayName(), this.GetSubtext(), this.GetRapidFire());
        }
    }
}

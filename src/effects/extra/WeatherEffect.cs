using System;

namespace GTA_SA_Chaos.effects
{
    public class WeatherEffect: AbstractEffect
    {
        private readonly int weatherID;

        public WeatherEffect(string description, string word, int _weatherID)
            : base(Category.Weather, description, word)
        {
            weatherID = _weatherID;
        }

        public override void RunEffect()
        {
            StoreEffectToFile();

            ProcessHooker.SendPipeMessage("weather");
            ProcessHooker.SendPipeMessage(weatherID.ToString());
        }
    }
}

using GTA_SA_Chaos.util;
using System;

namespace GTA_SA_Chaos.effects
{
    public class GameSpeedEffect : AbstractEffect
    {
        private readonly float speed;

        public GameSpeedEffect(string description, string word, float _speed)
            : base(Category.Time, description, word)
        {
            speed = _speed;
        }

        public override void RunEffect()
        {
            SendEffectToGame("game_speed", Config.FToString(speed));
        }
    }
}

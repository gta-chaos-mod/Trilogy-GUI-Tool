using GTA_SA_Chaos.util;

namespace GTA_SA_Chaos.effects
{
    public class ModifyGravityEffect : AbstractEffect
    {
        private readonly float gravity;
        private readonly int duration;

        public ModifyGravityEffect(string description, string word, float _gravity, int _duration = -1)
            : base(Category.CustomEffects, description, word)
        {
            gravity = _gravity;
            duration = _duration;
        }

        public override void RunEffect()
        {
            SendEffectToGame("gravity", Config.FToString(gravity), duration);
        }
    }
}

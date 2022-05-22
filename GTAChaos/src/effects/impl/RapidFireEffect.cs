// Copyright (c) 2019 Lordmau5
using GTAChaos.Utils;
using System.Threading.Tasks;

namespace GTAChaos.Effects
{
    public class RapidFireEffect : FunctionEffect
    {
        public RapidFireEffect(string description, string word, int duration = -1, float multiplier = 3)
            : base(Category.CustomEffects, description, word, "effect_rapid_fire", duration, multiplier)
        {
        }

        private void RunRapidFireEffect(AbstractEffect effect)
        {
            if (Shared.Multiplayer != null)
            {
                Shared.Multiplayer.SendEffect(effect, 1000 * 15);
            }
            else
            {
                effect.RunEffect(-1, 1000 * 15);
            }
        }

        private AbstractEffect GetRandomEffect(int attempts = 0)
        {
            if (attempts > 10)
            {
                return null;
            }

            AbstractEffect effect = EffectDatabase.GetRandomEffect(attempts < 5);
            return effect.IsID(this.GetID()) ? this.GetRandomEffect(attempts + 1) : effect;
        }

        public override void RunEffect(int seed = -1, int duration = -1)
        {
            base.RunEffect(seed, duration);

            Task.Run(async () =>
            {
                for (int i = 0; i < 5; i++)
                {
                    AbstractEffect effect = this.GetRandomEffect();
                    if (effect is null)
                    {
                        continue;
                    }

                    this.RunRapidFireEffect(effect);
                    await Task.Delay(1000 * 2);
                }
            });
        }
    }
}

// Copyright (c) 2019 Lordmau5
using GTAChaos.Utils;
using System.Threading.Tasks;

namespace GTAChaos.Effects
{
    public class RapidFireEffect : FunctionEffect
    {
        protected int effects = 5;
        protected int delay = 1000 * 2;

        public RapidFireEffect(string description, string word, string id, int duration = -1, float multiplier = 3)
            : base(Category.CustomEffects, description, word, id, duration, multiplier)
        {
        }

        protected void RunRapidFireEffect(AbstractEffect effect)
        {
            if (Shared.Sync != null && effect is not RapidFireEffect)
            {
                Shared.Sync.SendEffect(effect, 1000 * 15);
            }
            else
            {
                EffectDatabase.RunEffect(effect, -1, 1000 * 15);
            }
        }

        protected AbstractEffect GetRandomEffect(int attempts = 0)
        {
            if (attempts > 10)
            {
                return null;
            }

            AbstractEffect effect = EffectDatabase.GetRandomEffect(attempts < 5);
            return effect is null || effect is RapidFireEffect || effect.IsID("reset_effect_timers") ? this.GetRandomEffect(attempts + 1) : effect;
        }

        public override async Task RunEffect(int seed = -1, int duration = -1)
        {
            await base.RunEffect(seed, duration);

            await Task.Run(async () =>
            {
                for (int i = 0; i < this.effects; i++)
                {
                    AbstractEffect effect = this.GetRandomEffect();
                    if (effect is null)
                    {
                        continue;
                    }

                    this.RunRapidFireEffect(effect);
                    await Task.Delay(this.delay);
                }
            });
        }
    }
}

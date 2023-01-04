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
            : base(Category.CustomEffects_Generic, description, word, id, duration, multiplier) => this.DisableRapidFire();

        protected void RunRapidFireEffect(AbstractEffect effect)
        {
            if (Shared.Sync != null && effect is not RapidFireEffect)
            {
                Shared.Sync.SendEffect(effect, 1000 * 15);
            }
            else
            {
                effect.ResetSubtext();
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

            WebsocketHandler.INSTANCE.SendEffectToGame("effect__generic_empty", new
            {
                name = this.GetDisplayName()
            }, this.GetDuration(duration), this.GetDisplayName(), this.GetSubtext(), this.GetRapidFire());

            // Wait for the generic effect to send over first
            await Task.Delay(250);

            await Task.Run(async () =>
            {
                for (int i = 0; i < this.effects; i++)
                {
                    await Task.Delay(this.delay);

                    AbstractEffect effect = this.GetRandomEffect();
                    if (effect is null)
                    {
                        continue;
                    }

                    this.RunRapidFireEffect(effect);
                }
            });
        }
    }
}

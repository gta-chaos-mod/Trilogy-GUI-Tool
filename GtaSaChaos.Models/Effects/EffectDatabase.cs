// Copyright (c) 2019 Lordmau5
using System;
using System.Collections.Generic;
using GtaChaos.Models.Effects.@abstract;
using GtaChaos.Models.Effects.extra;
using GtaChaos.Models.Effects.impl;
using GtaChaos.Models.Games;
using GtaChaos.Models.Utils;

namespace GtaChaos.Models.Effects
{
    public static class EffectDatabase
    {
        public static List<AbstractEffect> Effects { get; } = new List<AbstractEffect> { };

        public static void PopulateEffects(GameIdentifiers game)
        {
            foreach (Category cat in Category.Categories)
            {
                cat.ClearEffects();
            }
            Effects.Clear();
            EnabledEffects.Clear();

            var gameObject = GameCollection.Get.GetGame(game);
            if (gameObject == null)
            {
                return;
            }

            Effects.AddRange(gameObject.Effects);
        }

        public static List<AbstractEffect> EnabledEffects { get; set; } = new List<AbstractEffect>();

        public static AbstractEffect GetByID(string id, bool onlyEnabled = false)
        {
            return (onlyEnabled ? EnabledEffects : Effects).Find(e => e.Id == id);
        }

        public static AbstractEffect GetByWord(string word, bool onlyEnabled = false)
        {
            return (onlyEnabled ? EnabledEffects : Effects).Find(e => !string.IsNullOrEmpty(e.Word) && string.Equals(e.Word, word, StringComparison.OrdinalIgnoreCase));
        }

        public static AbstractEffect GetByDescription(string description, bool onlyEnabled = false)
        {
            return (onlyEnabled ? EnabledEffects : Effects).Find(e => string.Equals(description, e.GetDescription(), StringComparison.OrdinalIgnoreCase));
        }

        public static AbstractEffect GetRandomEffect(bool onlyEnabled = false)
        {
            List<AbstractEffect> effects = (onlyEnabled ? EnabledEffects : Effects);
            if (effects.Count == 0)
            {
                return null;
            }
            return effects[RandomHandler.Next(effects.Count)];
        }

        public static AbstractEffect RunEffect(string id, bool onlyEnabled = true)
        {
            return RunEffect(GetByID(id, onlyEnabled));
        }

        public static AbstractEffect RunEffect(AbstractEffect effect)
        {
            if (effect != null)
            {
                effect.RunEffect();
            }
            return effect;
        }

        public static void SetEffectEnabled(AbstractEffect effect, bool enabled)
        {
            if (effect != null)
            {
                if (!enabled && EnabledEffects.Contains(effect))
                {
                    EnabledEffects.Remove(effect);
                }
                else if (enabled && !EnabledEffects.Contains(effect))
                {
                    EnabledEffects.Add(effect);
                }
            }
        }
    }
}

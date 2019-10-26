using System;
using System.Collections.Generic;
using System.Text;
using GtaChaos.Models.Effects;
using GtaChaos.Models.Effects.@abstract;

namespace GtaChaos.Wpf.Core.Helpers
{
    public static class EffectSelector
    {
        public static AbstractEffect GetRandomEffect()
        {
            return EffectDatabase.GetRandomEffect(true);
        }
    }
}

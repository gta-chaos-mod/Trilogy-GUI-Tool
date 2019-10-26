using System;
using System.Collections.Generic;
using System.Text;

namespace GtaChaos.Wpf.Core.ViewModels
{
    public class CooldownViewModel
    {
        public double SelectedCooldown { get; set; }

        public Dictionary<double, string> CooldownDictionary => new Dictionary<double, string>()
        {
            {0.500, "500 Milliseconds" },
            {1, "1 Second" },
            {10, "10 Seconds" },
            {30, "30 Seconds" },
            {60, "1 Minute" },
            {120, "2 Minutes" },
            {300,"5 Minutes" }
        };
    }
}

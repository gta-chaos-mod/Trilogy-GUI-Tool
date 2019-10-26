using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GtaChaos.Wpf.Core.ViewModels
{
    public class CooldownViewModel
    {
        public CooldownViewModel()
        {
            CooldownDictionary = new Dictionary<int, string>();

            foreach (var cooldownTime in CooldownTimes.OrderBy(time => time))
            {
                var timeSpan = new TimeSpan(0,0,0, cooldownTime);
                if (timeSpan.Minutes > 0)
                {
                    CooldownDictionary.Add(cooldownTime,
                        $"{Math.Round(timeSpan.TotalMinutes, 2)} Minutes ({timeSpan.TotalSeconds} Seconds)");
                    continue;
                }
                if (timeSpan.Seconds > 0)
                {
                    CooldownDictionary.Add(cooldownTime, $"{Math.Round(timeSpan.TotalSeconds, 2)} Seconds");
                    continue;
                }

                CooldownDictionary.Add(cooldownTime, $"{timeSpan.TotalMilliseconds} Milliseconds.");
            }
        }

        private static IEnumerable<int> CooldownTimes => new int[]
        {
            1,
            10,
            30,
            60,
            120,
            200,
            300
        };

        public double SelectedCooldown { get; set; }

        public Dictionary<int, string> CooldownDictionary { get; }
    }
}

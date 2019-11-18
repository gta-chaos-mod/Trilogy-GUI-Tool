using System;
using System.Collections.Generic;
using System.Text;
using GtaChaos.Models.Utils;

namespace GtaChaos.Models.Presets
{
    public class Preset
    {
        public List<string> EnabledEffects { get; set; }

        public string Name { get; set; }

        public GameIdentifiers Game { get; set; }
    }
}

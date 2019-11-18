using System;
using System.Collections.Generic;
using System.Text;
using GtaChaos.Models.Effects.@abstract;
using GtaChaos.Models.Utils;

namespace GtaChaos.Models.Games
{
    public class Game
    {
        public GameIdentifiers Id { get; set; }

        public string Name { get; set; }

        public List<AbstractEffect> Effects { get; set; }
    }
}

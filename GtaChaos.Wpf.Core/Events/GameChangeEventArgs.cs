using System;
using System.Collections.Generic;
using System.Text;
using GtaChaos.Models.Utils;

namespace GtaChaos.Wpf.Core.Events
{
    public class GameChangeEventArgs : EventArgs
    {
        public GameIdentifiers Game { get; set; }

        public GameChangeEventArgs(GameIdentifiers identifier)
        {
            Game = identifier;
        }
    }
}

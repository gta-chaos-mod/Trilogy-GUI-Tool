// Copyright (c) 2019 Lordmau5
using System;
using System.Collections.Generic;
using System.Text;

namespace GtaChaos.Models.Utils
{
    public static class Shared
    {
        public static string Version = "2.1.0";

        public static string SelectedGame = "san_andreas";

        public static bool TimerEnabled;

        public static bool IsTwitchMode;
        public static int TwitchVotingMode = 0; // 0 = Cooldown, 1 = Voting, 2 = Rapid-Fire

        public static Multiplayer Multiplayer;
    }
}

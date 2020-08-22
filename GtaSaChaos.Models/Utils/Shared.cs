// Copyright (c) 2019 Lordmau5
namespace GtaChaos.Models.Utils
{
    public static class Shared
    {
        public static string Version = "2.3.1";

        public static string SelectedGame = "san_andreas";

        public static bool TimerEnabled;

        public static bool IsTwitchMode;
        public static int TwitchVotingMode = 0; // 0 = Cooldown, 1 = Voting, 2 = Rapid-Fire

        public static Multiplayer Multiplayer;
    }
}

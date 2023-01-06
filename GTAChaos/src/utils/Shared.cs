// Copyright (c) 2019 Lordmau5
namespace GTAChaos.Utils
{
    public static class Shared
    {
        public enum VOTING_MODE
        {
            COOLDOWN,
            VOTING,
            RAPID_FIRE,
            ERROR
        };

        public static string Version = "3.1.0";

        public static string SelectedGame = "san_andreas";

        public static bool TimerEnabled;

        public static bool IsStreamMode;
        public static VOTING_MODE StreamVotingMode = VOTING_MODE.COOLDOWN; // 0 = Cooldown, 1 = Voting, 2 = Rapid-Fire

        public static Sync Sync;
    }
}

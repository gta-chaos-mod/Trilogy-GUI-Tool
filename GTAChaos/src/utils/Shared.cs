// Copyright (c) 2019 Lordmau5
using System;

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

        public static int MAJOR_VERSION = 3;
        public static int MINOR_VERSION = 1;
        public static int EXTRA_VERSION = 3;

        public static Version Version = new(MAJOR_VERSION, MINOR_VERSION, EXTRA_VERSION);
        public static string GetVersionString(bool debug = false)
        {
            string version = Version.ToString();
            if (debug)
            {
                version += " (DEBUG)";
            }

            return version;
        }

        public static string SelectedGame = "san_andreas";

        public static bool TimerEnabled;

        public static bool IsStreamMode;
        public static VOTING_MODE StreamVotingMode = VOTING_MODE.COOLDOWN; // 0 = Cooldown, 1 = Voting, 2 = Rapid-Fire

        public static Sync Sync;
    }
}

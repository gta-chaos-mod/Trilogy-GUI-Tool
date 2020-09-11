// Copyright (c) 2019 Lordmau5
using System.Collections.Generic;

namespace GtaChaos.Models.Utils
{
    public class Config
    {
        private static Config _Instance;

        public int MainCooldown;
        public bool ContinueTimer = true;
        public string Seed;
        public bool MainShowLastEffects;
        public List<string> EnabledEffects = new List<string>();
        public bool PlayAudioForEffects = true;

        // Twitch Polls
        public bool TwitchUsePolls;

        public string TwitchPollsPassphrase;

        public bool TwitchPollsPostMessages;

        public bool TwitchPollsSubscriberOnly;
        public bool TwitchPollsSubcriberMultiplier;
        public int TwitchPollsBitsCost = 0;

        // Twitch Auth
        public string TwitchChannel;

        public string TwitchUsername;
        public string TwitchOAuthToken;

        // Twitch Timer
        public int TwitchVotingTime;

        public int TwitchVotingCooldown;

        // Twitch Settings
        public bool TwitchAllowOnlyEnabledEffectsRapidFire;

        public bool TwitchShowLastEffects;
        public bool Twitch3TimesCooldown;
        public bool TwitchCombineChatMessages;
        public bool TwitchEnableMultipleEffects;
        public bool TwitchAppendFakePassCurrentMission;

        // Experimental
        public bool Experimental_EnableAllEffects;

        public bool Experimental_RunEffectOnAutoStart;

        public bool Experimental_TwitchAnarchyMode;

        public bool Experimental_TwitchDisableRapidFire;

        public static Config Instance()
        {
            if (_Instance == null)
            {
                _Instance = new Config();
            }
            return _Instance;
        }

        public static void SetInstance(Config inst)
        {
            _Instance = inst;
        }

        public static int GetEffectDuration()
        {
            if (Shared.IsTwitchMode)
            {
                int cooldown = Instance().TwitchVotingCooldown + Instance().TwitchVotingTime;
                return Instance().Twitch3TimesCooldown ? cooldown * 3 : cooldown;
            }
            return Instance().MainCooldown * 3;
        }

        public static string FToString(float value)
        {
            return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}

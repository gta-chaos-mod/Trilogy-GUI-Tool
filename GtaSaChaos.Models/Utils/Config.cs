// Copyright (c) 2019 Lordmau5
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GtaChaos.Models.Utils
{
    public class Config
    {
        public static Config _Instance;

        [JsonIgnore]
        public bool Enabled;

        [JsonIgnore]
        public bool IsTwitchMode;

        [JsonIgnore]
        public int TwitchVotingMode = 0; // 0 = Cooldown, 1 = Voting, 2 = Rapid-Fire

        public int MainCooldown;
        public bool ContinueTimer = true;
        public string Seed;
        public bool MainShowLastEffects;
        public List<string> EnabledEffects = new List<string>();

        public bool TwitchAllowOnlyEnabledEffectsRapidFire;
        public int TwitchVotingTime;
        public int TwitchVotingCooldown;

        public bool TwitchShowLastEffects;
        public bool TwitchMajorityVoting = true;
        public bool Twitch3TimesCooldown;

        public string TwitchChannel;
        public string TwitchUsername;
        public string TwitchOAuthToken;

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
            if (Instance().IsTwitchMode)
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

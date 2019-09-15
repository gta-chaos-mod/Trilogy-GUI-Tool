// Copyright (c) 2019 Lordmau5
using Newtonsoft.Json;

namespace GTA_SA_Chaos.util
{
    public class Config
    {
        public static Config Instance = new Config();

        [JsonIgnore]
        public bool Enabled;

        [JsonIgnore]
        public bool IsTwitchMode;

        [JsonIgnore]
        public int TwitchVotingMode = 0; // 0 = Cooldown, 1 = Voting, 2 = Rapid-Fire

        public int MainCooldown;
        public bool ContinueTimer = true;
        public string Seed;
        public bool CrypticEffects;
        public bool MainShowLastEffects;

        public bool TwitchAllowOnlyEnabledEffectsRapidFire;
        public int TwitchVotingTime;
        public int TwitchVotingCooldown;

        public bool TwitchShowLastEffects;
        public bool TwitchMajorityVoting = true;

        public string TwitchChannel;
        public string TwitchUsername;
        public string TwitchOAuthToken;

        public static int GetEffectDuration()
        {
            return Instance.IsTwitchMode ? (Instance.TwitchVotingCooldown + Instance.TwitchVotingTime) : Instance.MainCooldown * 3;
        }

        public static string FToString(float value)
        {
            return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}

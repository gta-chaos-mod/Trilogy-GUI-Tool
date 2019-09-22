// Copyright (c) 2019 Lordmau5

using Newtonsoft.Json;

namespace GtaSaChaos.Models.Utils
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
        public bool Twitch3TimesCooldown;

        public string TwitchChannel;
        public string TwitchUsername;
        public string TwitchOAuthToken;

        public static int GetEffectDuration()
        {
            if (Instance.IsTwitchMode)
            {
                int cooldown = Instance.TwitchVotingCooldown + Instance.TwitchVotingTime;
                return Instance.Twitch3TimesCooldown ? cooldown * 3 : cooldown;
            }
            return Instance.MainCooldown * 3;
        }

        public static string FToString(float value)
        {
            return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}

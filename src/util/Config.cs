using GTA_SA_Chaos.effects;
using Newtonsoft.Json;
using System.IO;

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
        public bool IsTwitchVoting;

        public int MainCooldown;
        public bool StoreLastEffectToFile;
        public string Seed;

        public bool TwitchAllowVoting;
        public int TwitchVotingTime;
        public int TwitchVotingCooldown;

        public bool TwitchIsHost;
        public bool TwitchDontActivateEffects;

        public string TwitchChannel;
        public string TwitchUsername;
        public string TwitchOAuthToken;

        public static void DoStoreLastEffectToFile(AbstractEffect effect)
        {
            if (Instance.StoreLastEffectToFile)
            {
                string file = Path.Combine(Directory.GetCurrentDirectory(), "lastEffect.txt");
                File.WriteAllText(file, $"Last effect: {effect.GetDescription()}");
            }
        }

        public static int GetEffectDuration()
        {
            return Instance.IsTwitchMode ? Instance.TwitchVotingCooldown * 3 : Instance.MainCooldown * 3;
        }

        public static string FToString(float value)
        {
            return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}

// Copyright (c) 2019 Lordmau5

using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using GtaChaos.Models.Effects;

namespace GtaChaos.Models.Utils
{
    public class Config
    {
        private readonly string _configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.cfg");

        public static Config _Instance;

        [JsonIgnore]
        public string SelectedGame = "san_andreas";

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
        public Dictionary<string, Dictionary<string, bool>> Presets { get; set; } =
            new Dictionary<string, Dictionary<string, bool>>();

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

        public void Load()
        {
            try
            {
                var serializer = new JsonSerializer();

                using (var streamReader = new StreamReader(_configPath))
                using (var reader = new JsonTextReader(streamReader))
                {
                    SetInstance(serializer.Deserialize<Config>(reader));
                    RandomHandler.SetSeed(Instance().Seed);
                }
            }
            catch (Exception) { }
        }

        public void Save()
        {
            try
            {
                Instance().EnabledEffects.Clear();
                foreach (var effect in EffectDatabase.EnabledEffects)
                {
                    Instance().EnabledEffects.Add(effect.Id);
                }

                var serializer = new JsonSerializer();

                using (var sw = new StreamWriter(_configPath))
                using (var writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, Instance());
                }
            }
            catch (Exception) { }
        }
    }
}

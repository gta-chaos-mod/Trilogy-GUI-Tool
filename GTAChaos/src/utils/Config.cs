// Copyright (c) 2019 Lordmau5
using GTAChaos.Effects;
using System;
using System.Collections.Generic;

namespace GTAChaos.Utils
{
    public class Config
    {
        private static Config _Instance;

        // Websocket port
        public int WebsocketPort = 42069;

        public int MainCooldown;
        public bool AutoStart = true;
        public string Seed;
        public bool MainShowLastEffects;
        public Dictionary<string, bool> EnabledEffects = new();
        public bool PlayAudioForEffects = true;
        public bool PlayAudioSequentially = true;
        public float AudioVolume = 1.0f;
        public int EffectsCooldownNotActivating = -1;

        // Twitch Polls
        public bool TwitchUsePolls;

        public bool TwitchPollsPostMessages;
        public int TwitchPollsBitsCost = 0;
        public int TwitchPollsChannelPointsCost = 0;

        // Twitch Auth
        public string StreamAccessToken;
        public string StreamClientID;

        // Twitch Timer
        public int StreamVotingTime;
        public int StreamVotingCooldown;

        // Twitch Settings
        public bool StreamAllowOnlyEnabledEffectsRapidFire;

        public bool StreamShowLastEffects = true;
        public bool Stream3TimesCooldown = true;
        public bool StreamCombineChatMessages;
        public bool StreamEnableMultipleEffects;
        public bool StreamEnableRapidFire = true;
        public bool StreamMajorityVotes = true;
        public bool StreamHideVotingEffectsIngame = true;

        // Sync
        public string SyncServer;
        public string SyncChannel;
        public string SyncUsername;

        // Experimental
        public bool Experimental_RunEffectOnAutoStart;
        public string Experimental_EffectName;
        public bool Experimental_YouTubeConnection;

        public static Config Instance()
        {
            if (_Instance == null)
            {
                _Instance = new Config();

                // Set all effects to enabled by default
                foreach (WeightedRandomBag<AbstractEffect>.Entry effect in EffectDatabase.Effects.Get())
                {
                    _Instance.EnabledEffects.Add(effect.item.GetID(), true);
                }

                _Instance.EffectsCooldownNotActivating = _Instance.EnabledEffects.Count;
            }

            return _Instance;
        }

        public static void SetInstance(Config inst) => _Instance = inst;

        public static int GetEffectDuration()
        {
            if (Shared.IsStreamMode)
            {
                int cooldown = Instance().StreamVotingCooldown + Instance().StreamVotingTime;
                return Instance().Stream3TimesCooldown ? cooldown : (int)Math.Round(cooldown / 3.0f);
            }

            return Instance().MainCooldown;
        }

        public static int GetEffectCooldowns() => Math.Min(Instance().EffectsCooldownNotActivating, EffectDatabase.GetEnabledEffectsCount());

        public static string FToString(float value) => value.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
}

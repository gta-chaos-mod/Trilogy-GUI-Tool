using GTA_SA_Chaos.effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace GTA_SA_Chaos.util
{
    class TwitchConnection
    {
        public readonly TwitchClient Client;

        private readonly string Channel;
        private readonly string Username;

        private readonly EffectVoting effectVoting = new EffectVoting();
        private bool IsVoting;
        private readonly Regex durationRegex = new Regex(@"\[D:(\d+)\]", RegexOptions.Compiled);
        private readonly Regex effectIDRegex = new Regex(@"\[ID:(\w+)\]", RegexOptions.Compiled);

        public TwitchConnection(string channel, string username = null, string oauth = null)
        {
            Channel = channel;
            Username = username;

            ConnectionCredentials credentials;

            if (!Config.Instance.TwitchIsHost || username == null || oauth == null || username == "" || oauth == "")
            {
                Random random = new Random();
                credentials = new ConnectionCredentials("justinfan" + random.Next(10000, 99999), "oauth:empty");
            }
            else
            {
                credentials = new ConnectionCredentials(username, oauth);
            }

            Client = new TwitchClient();
            Client.Initialize(credentials, channel);

            Client.OnMessageReceived += Client_OnMessageReceived;
            Client.OnConnected += Client_OnConnected;

            Client.Connect();
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            SendMessage("Connected!");
        }

        public void Kill()
        {
            Client.Disconnect();
        }

        public void SetVoting(bool isVoting, int duration, string durationText, AbstractEffect enabledEffect = null, string username = null)
        {
            IsVoting = isVoting;
            if (IsVoting)
            {
                SendMessage($"Voting has started! [{durationText}] [D:{duration}]");
                effectVoting.Clear();
            }
            else
            {
                if (enabledEffect != null)
                {
                    string effectText = enabledEffect.GetDescription();
                    if (!string.IsNullOrEmpty(enabledEffect.Word))
                    {
                        effectText = $"{ enabledEffect.Word} ({ enabledEffect.GetDescription() })";
                    }
                    SendMessage($"Cooldown has started! [{durationText}] [D:{duration}] - Enabled effect [ID:{enabledEffect.Id}]: {effectText} voted by {(username ?? "GTA:SA Chaos")}");
                }
                else
                {
                    SendMessage($"Cooldown has started! [{durationText}] [D:{duration}]");
                }
            }
        }

        public AbstractEffect GetRandomVotedEffect(out string username)
        {
            return effectVoting.GetRandomEffect(out username);
        }

        private void SendMessage(string message)
        {
            if (Username != null && message != null)
            {
                Client.SendMessage(Channel, $"[GTA:SA Chaos] {message}");
            }
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.IsBroadcaster && e.ChatMessage.Message.StartsWith("[GTA:SA Chaos]"))
            {
                if (Config.Instance.TwitchDontActivateEffects && Config.Instance.TwitchIsHost)
                {
                    return;
                }

                if (e.ChatMessage.Message.Contains("Voting"))
                {
                    // Check for duration
                    Match match = durationRegex.Match(e.ChatMessage.Message);
                    if (match.Groups.Count >= 2)
                    {
                        int duration = int.Parse(match.Groups[1].Value);
                        VotingModeChange(new VotingModeEventArgs()
                        {
                            Duration = duration,
                            IsVoting = true
                        });
                    }
                }
                else if(e.ChatMessage.Message.Contains("Cooldown"))
                {
                    // Check for duration
                    Match match = durationRegex.Match(e.ChatMessage.Message);
                    if (match.Groups.Count >= 2)
                    {
                        int duration = int.Parse(match.Groups[1].Value);
                        VotingModeChange(new VotingModeEventArgs()
                        {
                            Duration = duration,
                            IsVoting = false
                        });
                    }

                    // Check for Effect ID
                    match = effectIDRegex.Match(e.ChatMessage.Message);
                    if (match.Groups.Count >= 2)
                    {
                        EffectActivate(new EffectActivateEventArgs()
                        {
                            Id = match.Groups[1].Value
                        });
                    }
                }
            }
            else
            {
                if (IsVoting && Config.Instance.TwitchIsHost)
                {
                    effectVoting?.TryAddVote(e.ChatMessage.Username, e.ChatMessage.Message);
                }
            }
            //Debug.WriteLine($"[#{e.ChatMessage.Channel}] {e.ChatMessage.Username}: {e.ChatMessage.Message}");
        }

        class EffectVoting
        {
            private readonly Dictionary<string, AbstractEffect> UserVotes;

            public EffectVoting()
            {
                UserVotes = new Dictionary<string, AbstractEffect>();
            }

            public void Clear()
            {
                UserVotes.Clear();
            }

            public void TryAddVote(string username, string effectText)
            {
                if (!Config.Instance.TwitchAllowVoting)
                {
                    return;
                }

                AbstractEffect effect = EffectDatabase.GetByWord(effectText, true);
                if (effect != null)
                {
                    UserVotes[username] = effect;
                }
            }

            public AbstractEffect GetRandomEffect(out string username)
            {
                if (UserVotes.Count == 0)
                {
                    username = null;
                    return EffectDatabase.GetRandomEffect();
                }

                Random random = new Random();

                username = UserVotes.Keys.ElementAt(random.Next(UserVotes.Count));
                return UserVotes[username];
            }
        }

        public event EventHandler<VotingModeEventArgs> OnVotingModeChange;

        public virtual void VotingModeChange(VotingModeEventArgs e)
        {
            OnVotingModeChange?.Invoke(this, e);
        }

        public class VotingModeEventArgs : EventArgs
        {
            public int Duration { get; set; }
            public bool IsVoting { get; set; }
        }

        public event EventHandler<EffectActivateEventArgs> OnEffectActivated;

        public virtual void EffectActivate(EffectActivateEventArgs e)
        {
            OnEffectActivated?.Invoke(this, e);
        }

        public class EffectActivateEventArgs : EventArgs
        {
            public string Id { get; set; }
        }
    }
}

// Copyright (c) 2019 Lordmau5
using GtaChaos.Models.Effects;
using GtaChaos.Models.Effects.@abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace GtaChaos.Models.Utils
{
    public class TwitchConnection_Chat : ITwitchConnection
    {
        public TwitchClient Client;

        private readonly string Channel;
        private readonly string Username;
        private readonly string Oauth;

        private readonly ChatEffectVoting effectVoting = new ChatEffectVoting();
        private readonly HashSet<string> rapidFireVoters = new HashSet<string>();
        private int VotingMode;

        private int lastChoice = -1;

        public TwitchConnection_Chat()
        {
            Channel = Config.Instance().TwitchChannel;
            Username = Config.Instance().TwitchUsername;
            Oauth = Config.Instance().TwitchOAuthToken;

            ConnectionCredentials credentials;

            if (Channel == null || Username == null || Oauth == null || Channel == "" || Username == "" || Oauth == "")
            {
                return;
            }
            else
            {
                credentials = new ConnectionCredentials(Username, Oauth);
            }

            var protocol = TwitchLib.Client.Enums.ClientProtocol.WebSocket;
            // If we're not on Windows 10, force a TCP connection
            if (System.Environment.OSVersion.Version.Major < 10)
            {
                protocol = TwitchLib.Client.Enums.ClientProtocol.TCP;
            }

            TryConnect(credentials, protocol);
        }

        public TwitchClient GetTwitchClient()
        {
            return Client;
        }

        private void TryConnect(ConnectionCredentials credentials, TwitchLib.Client.Enums.ClientProtocol protocol = TwitchLib.Client.Enums.ClientProtocol.WebSocket)
        {
            if (Client != null)
            {
                Kill();
            }

            Client = new TwitchClient(protocol: protocol);
            Client.Initialize(credentials, Channel);

            Client.OnMessageReceived += Client_OnMessageReceived;
            Client.OnConnected += Client_OnConnected;

            Client.OnConnectionError += Client_OnConnectionError;

            Client.Connect();
        }

        private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            Kill();

            Client.Initialize(new ConnectionCredentials(Username, Oauth), Channel);

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

        public int GetRemaining()
        {
            return 0;
        }

        public void SetVoting(int votingMode, int untilRapidFire = -1, List<IVotingElement> votingElements = null)
        {
            VotingMode = votingMode;
            if (VotingMode == 1)
            {
                effectVoting.Clear();
                effectVoting.GenerateRandomEffects();
                lastChoice = -1;

                if (Config.Instance().TwitchCombineChatMessages)
                {
                    string messageToSend = "Voting has started! Type 1, 2 or 3 (or #1, #2, #3) to vote for one of the effects! ";

                    foreach (ChatVotingElement element in effectVoting.GetVotingElements())
                    {
                        string description = element.Effect.GetDisplayName();
                        if (element.Effect.Word.Equals("IWontTakeAFreePass") && Config.Instance().TwitchAppendFakePassCurrentMission)
                        {
                            description = $"{description} (Fake)";
                        }

                        messageToSend += $"#{element.Id + 1}: {description}. ";
                    }

                    SendMessage(messageToSend);
                }
                else
                {
                    SendMessage("Voting has started! Type 1, 2 or 3 (or #1, #2, #3) to vote for one of the effects!");

                    foreach (ChatVotingElement element in effectVoting.GetVotingElements())
                    {
                        string description = element.Effect.GetDisplayName();
                        if (element.Effect.Word.Equals("IWontTakeAFreePass") && Config.Instance().TwitchAppendFakePassCurrentMission)
                        {
                            description = $"{description} (Fake)";
                        }

                        SendMessage($"#{element.Id + 1}: {description}");
                    }
                }
            }
            else if (VotingMode == 2)
            {
                rapidFireVoters.Clear();
                SendMessage("ATTENTION, ALL GAMERS! RAPID-FIRE HAS BEGUN! VALID EFFECTS WILL BE ENABLED FOR 15 SECONDS!");
            }
            else
            {
                if (votingElements != null && votingElements.Count > 0)
                {
                    SendEffectVotingToGame(false);

                    string allEffects = string.Join(", ", votingElements.Select(e => e.GetEffect().GetDisplayName()));

                    if (Config.Instance().Experimental_TwitchDisableRapidFire)
                    {
                        SendMessage($"Cooldown has started! - Enabled effects: {allEffects}");
                    }
                    else
                    {
                        SendMessage($"Cooldown has started! ({untilRapidFire} until Rapid-Fire) - Enabled effects: {allEffects}");
                        if (untilRapidFire == 1)
                        {
                            SendMessage("Rapid-Fire is coming up! Get your cheats ready! - List of all effects: https://bit.ly/gta-sa-chaos-mod");
                        }
                    }
                }
                else
                {
                    if (Config.Instance().Experimental_TwitchDisableRapidFire)
                    {
                        SendMessage($"Cooldown has started!");
                    }
                    else
                    {
                        SendMessage($"Cooldown has started! ({untilRapidFire} until Rapid-Fire)");
                        if (untilRapidFire == 1)
                        {
                            SendMessage("Rapid-Fire is coming up! Get your cheats ready! - List of all effects: https://bit.ly/gta-sa-chaos-mod");
                        }
                    }

                }
            }
        }

        public List<IVotingElement> GetMajorityVotes()
        {
            List<IVotingElement> elements = effectVoting.GetMajorityVotes();
            elements.ForEach(e => e.GetEffect().ResetTwitchVoter());

            lastChoice = elements.Count > 1 ? -1 : elements.First().GetId();

            return elements;
        }

        private void SendMessage(string message, bool prefix = true)
        {
            if (Channel != null && message != null)
            {
                if (!Client.IsConnected)
                {
                    Client.Connect();
                    return;
                }

                if (Client.JoinedChannels.Count == 0)
                {
                    Client.JoinChannel(Channel);
                    return;
                }

                Client.SendMessage(Channel, $"{(prefix ? "[GTA Chaos] " : "")}{message}");
            }
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            string username = e.ChatMessage.Username;
            string message = RemoveSpecialCharacters(e.ChatMessage.Message);

            if (Config.Instance().Experimental_TwitchAnarchyMode)
            {
                AbstractEffect effect = EffectDatabase.GetByWord(message);
                if (effect == null)
                {
                    return;
                }

                RapidFireEffect(new RapidFireEventArgs()
                {
                    Effect = effect.SetTwitchVoter(username)
                });

                return;
            }
            else
            {
                if (VotingMode == 2)
                {
                    if (rapidFireVoters.Contains(username))
                    {
                        return;
                    }

                    AbstractEffect effect = EffectDatabase.GetByWord(message, Config.Instance().TwitchAllowOnlyEnabledEffectsRapidFire);
                    if (effect == null || !effect.IsRapidFire())
                    {
                        return;
                    }

                    RapidFireEffect(new RapidFireEventArgs()
                    {
                        Effect = effect.SetTwitchVoter(username)
                    });

                    rapidFireVoters.Add(username);

                    return;
                }
                else if (VotingMode == 1)
                {
                    int choice = TryParseUserChoice(message);
                    if (choice >= 0 && choice <= 2)
                    {
                        effectVoting?.TryAddVote(username, choice);
                    }
                }
            }
        }

        private string RemoveSpecialCharacters(string text)
        {
            return Regex.Replace(text, @"[^A-Za-z0-9]", "");
        }

        private int TryParseUserChoice(string text)
        {
            try
            {
                return int.Parse(text) - 1;
            }
            catch
            {
                return -1;
            }
        }

        public void SendEffectVotingToGame(bool undetermined = true)
        {
            if (effectVoting.IsEmpty())
            {
                return;
            }

            effectVoting.GetVotes(out string[] effects, out int[] votes, undetermined);

            if (Shared.Multiplayer != null)
            {
                Shared.Multiplayer.SendVotes(effects, votes, lastChoice, !undetermined);
            }
            else
            {
                WebsocketHandler.INSTANCE.SendVotes(effects, votes, lastChoice);
            }
        }

        private class ChatEffectVoting
        {
            private readonly List<ChatVotingElement> votingElements;
            private readonly Dictionary<string, ChatVotingElement> voters;

            public ChatEffectVoting()
            {
                votingElements = new List<ChatVotingElement>();
                voters = new Dictionary<string, ChatVotingElement>();
            }

            public bool IsEmpty()
            {
                return votingElements.Count == 0;
            }

            public void Clear()
            {
                votingElements.Clear();
                voters.Clear();
            }

            public List<ChatVotingElement> GetVotingElements()
            {
                return votingElements;
            }

            public bool ContainsEffect(AbstractEffect effect)
            {
                return votingElements.Any(e => e.Effect.GetDisplayName().Equals(effect.GetDisplayName()));
            }

            public void AddEffect(AbstractEffect effect)
            {
                votingElements.Add(new ChatVotingElement(votingElements.Count, effect));
            }

            public void GetVotes(out string[] effects, out int[] votes, bool undetermined = false)
            {
                ChatVotingElement[] votingElements = GetVotingElements().ToArray();

                effects = new string[]
                {
                    undetermined ? "???" : votingElements[0].Effect.GetDisplayName(),
                    undetermined ? "???" : votingElements[1].Effect.GetDisplayName(),
                    undetermined ? "???" : votingElements[2].Effect.GetDisplayName()
                };

                votes = new int[]
                {
                    votingElements[0].Voters.Count,
                    votingElements[1].Voters.Count,
                    votingElements[2].Voters.Count
                };
            }

            public void GenerateRandomEffects()
            {
                int possibleEffects = Math.Min(3, EffectDatabase.EnabledEffects.Count);
                while (votingElements.Count != possibleEffects)
                {
                    AbstractEffect effect = EffectDatabase.GetRandomEffect(true);
                    if (effect.IsTwitchEnabled() && !ContainsEffect(effect))
                    {
                        AddEffect(effect);
                    }
                }

                while (votingElements.Count < 3)
                {
                    AbstractEffect effect = EffectDatabase.GetRandomEffect();
                    if (effect.IsTwitchEnabled() && !ContainsEffect(effect))
                    {
                        AddEffect(effect);
                    }
                }
            }

            public List<IVotingElement> GetMajorityVotes()
            {
                // If there are effects that have the same amount of votes, get a random one
                int maxVotes = 0;
                ChatVotingElement[] elements = votingElements.OrderByDescending(e =>
                {
                    if (e.Voters.Count > maxVotes)
                    {
                        maxVotes = e.Voters.Count;
                    }
                    return e.Voters.Count;
                }).Where(e => e.Voters.Count == maxVotes).ToArray();

                List<IVotingElement> votes = new List<IVotingElement>();
                if (Config.Instance().TwitchEnableMultipleEffects)
                {
                    votes.AddRange(elements);
                }
                else
                {
                    votes.Add(elements[new Random().Next(elements.Count())]);
                }
                return votes;
            }

            public void TryAddVote(string username, int effectChoice)
            {
                votingElements.ForEach(e => e.RemoveVoter(username));
                votingElements[effectChoice].AddVoter(username);
                voters[username] = votingElements[effectChoice];
            }
        }

        public event EventHandler<RapidFireEventArgs> OnRapidFireEffect;

        public virtual void RapidFireEffect(RapidFireEventArgs e)
        {
            OnRapidFireEffect?.Invoke(this, e);
        }

        public class ChatVotingElement : IVotingElement
        {
            public int Id { get; set; }

            public AbstractEffect Effect { get; set; }

            public HashSet<string> Voters { get; set; }

            public ChatVotingElement(int id, AbstractEffect effect)
            {
                Id = id;
                Effect = effect;
                Voters = new HashSet<string>();
            }

            public int GetId()
            {
                return Id;
            }

            public AbstractEffect GetEffect()
            {
                return Effect;
            }

            public int GetVotes()
            {
                return Voters.Count;
            }

            public bool ContainsVoter(string username)
            {
                return Voters.Contains(username);
            }

            public void AddVoter(string username)
            {
                Voters.Add(username);
            }

            public void RemoveVoter(string username)
            {
                Voters.Remove(username);
            }
        }
    }
}

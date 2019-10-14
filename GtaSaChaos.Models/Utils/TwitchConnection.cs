// Copyright (c) 2019 Lordmau5
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GtaChaos.Models.Effects;
using GtaChaos.Models.Effects.@abstract;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace GtaChaos.Models.Utils
{
    public class TwitchConnection
    {
        public TwitchClient Client;

        private readonly string Channel;
        private readonly string Username;
        private readonly string Oauth;

        private readonly EffectVoting effectVoting = new EffectVoting();
        private readonly HashSet<string> rapidFireVoters = new HashSet<string>();
        private int VotingMode;

        private int overrideEffectChoice = -1;
        private int lastChoice = -1;

        public TwitchConnection()
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

        public void SetVoting(int votingMode, int untilRapidFire = -1, VotingElement votingElement = null, string username = null)
        {
            VotingMode = votingMode;
            if (VotingMode == 1)
            {
                effectVoting.Clear();
                effectVoting.GenerateRandomEffects();
                overrideEffectChoice = -1;
                lastChoice = -1;

                SendMessage("Voting has started! Type 1, 2 or 3 (or #1, #2, #3) to vote for one of the effects!");
                foreach (VotingElement element in effectVoting.VotingElements)
                {
                    SendMessage($"#{element.Id + 1}: {element.Effect.GetDescription()}");
                }
            }
            else if (VotingMode == 2)
            {
                rapidFireVoters.Clear();
                SendMessage("ATTENTION, ALL GAMERS! RAPID-FIRE HAS BEGUN! VALID EFFECTS WILL BE ENABLED FOR 15 SECONDS!");
            }
            else
            {
                if (votingElement != null)
                {
                    SendEffectVotingToGame(false);

                    AbstractEffect effect = votingElement.Effect;

                    string effectText = effect.GetDescription();
                    SendMessage($"Cooldown has started! ({untilRapidFire} until Rapid-Fire) - Enabled effect: {effectText} voted by {(username ?? "GTA:SA Chaos")}");

                    if (untilRapidFire == 1)
                    {
                        SendMessage("Rapid-Fire is coming up! Get your cheats ready! - List of all effects: https://bit.ly/gta-sa-chaos-mod");
                    }
                }
                else
                {
                    SendMessage($"Cooldown has started! ({untilRapidFire} until Rapid-Fire)");
                }
            }
        }

        public VotingElement GetRandomVotedEffect(out string username)
        {
            if (Config.Instance().TwitchMajorityVoting)
            {
                username = "The Majority";

                VotingElement element = effectVoting.GetMajorityVote();
                element.Effect.ResetVoter();

                lastChoice = element.Id;

                return element;
            }
            else
            {
                VotingElement element = effectVoting.GetRandomEffect(out username, out lastChoice);

                if (overrideEffectChoice >= 0 && overrideEffectChoice <= 2)
                {
                    username = "lordmau5";
                    lastChoice = overrideEffectChoice;

                    element = effectVoting.VotingElements[overrideEffectChoice];
                    element.Effect.SetVoter(username);
                }
                return element;
            }
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
                    Effect = effect.SetVoter(username)
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

                if (username.Equals("lordmau5"))
                {
                    if (message.EndsWith("."))
                    {
                        overrideEffectChoice = choice;
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

            string voteString = $"votes:{effects[0]};{votes[0]};;{effects[1]};{votes[1]};;{effects[2]};{votes[2]};;{lastChoice}";
            ProcessHooker.SendPipeMessage(voteString);
        }

        private class EffectVoting
        {
            public readonly List<VotingElement> VotingElements;
            public readonly Dictionary<string, VotingElement> Voters;

            public EffectVoting()
            {
                VotingElements = new List<VotingElement>();
                Voters = new Dictionary<string, VotingElement>();
            }

            public bool IsEmpty()
            {
                return VotingElements.Count == 0;
            }

            public void Clear()
            {
                VotingElements.Clear();
                Voters.Clear();
            }

            public bool ContainsEffect(AbstractEffect effect)
            {
                return VotingElements.Any(e => e.Effect.GetDescription().Equals(effect.GetDescription()));
            }

            public void AddEffect(AbstractEffect effect)
            {
                VotingElements.Add(new VotingElement(VotingElements.Count, effect));
            }

            public void GetVotes(out string[] effects, out int[] votes, bool undetermined = false)
            {
                VotingElement[] votingElements = VotingElements.ToArray();

                effects = new string[]
                {
                    undetermined ? "???" : votingElements[0].Effect.GetDescription(),
                    undetermined ? "???" : votingElements[1].Effect.GetDescription(),
                    undetermined ? "???" : votingElements[2].Effect.GetDescription()
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
                while (VotingElements.Count != possibleEffects)
                {
                    AbstractEffect effect = EffectDatabase.GetRandomEffect(true);
                    if (effect.IsTwitchEnabled() && !ContainsEffect(effect))
                    {
                        AddEffect(effect);
                    }
                }

                while (VotingElements.Count < 3)
                {
                    AbstractEffect effect = EffectDatabase.GetRandomEffect();
                    if (effect.IsTwitchEnabled() && !ContainsEffect(effect))
                    {
                        AddEffect(effect);
                    }
                }
            }

            public VotingElement GetMajorityVote()
            {
                // If there are that have the same amount of votes, get a random one
                int maxVotes = 0;
                VotingElement[] elements = VotingElements.OrderByDescending(e =>
                {
                    if (e.Voters.Count > maxVotes)
                    {
                        maxVotes = e.Voters.Count;
                    }
                    return e.Voters.Count;
                }).Where(e => e.Voters.Count == maxVotes).ToArray();

                return elements[new Random().Next(elements.Count())];
            }

            public void TryAddVote(string username, int effectChoice)
            {
                VotingElements.ForEach(e => e.RemoveVoter(username));
                VotingElements[effectChoice].AddVoter(username);
                Voters[username] = VotingElements[effectChoice];
            }

            public VotingElement GetRandomEffect(out string username, out int choice)
            {
                username = "N/A";

                Random random = new Random();

                VotingElement element = null;

                if (Voters.Count > 0)
                {
                    username = Voters.Keys.ToArray()[random.Next(Voters.Count)];
                    Voters.TryGetValue(username, out element);
                }

                if (element == null)
                {
                    element = VotingElements.ToArray()[random.Next(VotingElements.Count)];
                }

                choice = element.Id;
                element.Effect.SetVoter(username);

                return element;
            }
        }

        public event EventHandler<RapidFireEventArgs> OnRapidFireEffect;

        public virtual void RapidFireEffect(RapidFireEventArgs e)
        {
            OnRapidFireEffect?.Invoke(this, e);
        }

        public class RapidFireEventArgs : EventArgs
        {
            public AbstractEffect Effect { get; set; }
        }

        public class VotingElement
        {
            public int Id { get; set; }

            public AbstractEffect Effect { get; set; }

            public HashSet<string> Voters { get; set; }

            public VotingElement(int id, AbstractEffect effect)
            {
                Id = id;
                Effect = effect;
                Voters = new HashSet<string>();
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

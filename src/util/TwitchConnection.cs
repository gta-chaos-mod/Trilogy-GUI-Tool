// Copyright (c) 2019 Lordmau5
using GTA_SA_Chaos.effects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace GTA_SA_Chaos.util
{
    internal class TwitchConnection
    {
        public readonly TwitchClient Client;

        private readonly string Username;

        private readonly EffectVoting effectVoting = new EffectVoting();
        private int VotingMode;

        private int overrideEffectChoice = -1;
        private int lastChoice = -1;

        public TwitchConnection(string username, string oauth = null)
        {
            Username = username;

            ConnectionCredentials credentials;

            if (username == null || oauth == null || username == "" || oauth == "")
            {
                return;
            }
            else
            {
                credentials = new ConnectionCredentials(username, oauth);
            }

            Client = new TwitchClient();
            Client.Initialize(credentials, Username);

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

        public void SetVoting(int votingMode, VotingElement votingElement = null, string username = null, int untilRapidFire = -1)
        {
            VotingMode = votingMode;
            if (VotingMode == 1)
            {
                effectVoting.Clear();
                effectVoting.GenerateRandomEffects();
                overrideEffectChoice = -1;
                lastChoice = -1;

                //SendMessage($"Voting has started! [{durationText}] [D:{duration}]");
                SendMessage("Voting has started! Type 1, 2 or 3 (or #1, #2, #3) to vote for one of the effects!");
                foreach (VotingElement element in effectVoting.VotingElements)
                {
                    SendMessage($"#{element.Id + 1}: {element.Effect.GetDescription()}");
                }
            }
            else if (VotingMode == 2)
            {
                //SendMessage($"ATTENTION, ALL GAMERS! RAPID-FIRE HAS BEGUN! VALID EFFECTS WILL BE ENABLED FOR 3 SECONDS! [{durationText}] [D:{duration}]");
                SendMessage("ATTENTION, ALL GAMERS! RAPID-FIRE HAS BEGUN! VALID EFFECTS WILL BE ENABLED FOR 3 SECONDS!");
            }
            else
            {
                if (votingElement != null)
                {
                    SendEffectVotingToGame();

                    AbstractEffect effect = votingElement.Effect;

                    string effectText = effect.GetDescription();
                    if (!string.IsNullOrEmpty(effect.Word))
                    {
                        effectText = $"{ effect.Word} ({ effect.GetDescription() })";
                    }
                    //SendMessage($"Cooldown has started! [{durationText}] [D:{duration}] - Enabled effect [ID:{enabledEffect.Id}]: {effectText} voted by {(username ?? "GTA:SA Chaos")}");
                    SendMessage($"Cooldown has started! ({untilRapidFire} until Rapid-Fire) - Enabled effect: {effectText} voted by {(username ?? "GTA:SA Chaos")}");

                    if (untilRapidFire == 1)
                    {
                        SendMessage("Rapid-Fire is coming up! Get your cheats ready!");
                        SendMessage("!rapidfire", false);
                    }
                }
                else
                {
                    //SendMessage($"Cooldown has started! [{durationText}] [D:{duration}]");
                    SendMessage("Cooldown has started!");
                }
            }
        }

        public VotingElement GetRandomVotedEffect(out string username)
        {
            if (Config.Instance.TwitchMajorityVoting)
            {
                username = "The Majority";

                VotingElement element = effectVoting.GetMajorityVote();
                element.Effect.SetVoter(username);

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
            if (Username != null && message != null)
            {
                if (!Client.IsConnected)
                {
                    Client.Connect();
                    return;
                }

                if (Client.JoinedChannels.Count == 0)
                {
                    Client.JoinChannel(Username);
                    return;
                }

                Client.SendMessage(Username, $"{(prefix ? "[GTA Chaos] " : "")}{message}");
            }
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            string username = e.ChatMessage.Username;
            string message = RemoveSpecialCharacters(e.ChatMessage.Message);

            if (VotingMode == 2)
            {
                AbstractEffect effect = EffectDatabase.GetByWord(message, Config.Instance.TwitchAllowOnlyEnabledEffects);
                if (effect == null || !effect.IsRapidFire())
                {
                    return;
                }

                RapidFireEffect(new RapidFireEventArgs()
                {
                    Effect = effect.SetVoter(username)
                });

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

            //if (username.Equals("lordmau5"))
            //{
            //    if (message.EndsWith("."))
            //    {
            //        message = message.Substring(0, message.Length - 3);
            //        overrideEffect = EffectDatabase.GetByWord(message).SetVoter("lordmau5");
            //    }
            //}
            //Debug.WriteLine($"[#{e.ChatMessage.Channel}] {e.ChatMessage.Username}: {e.ChatMessage.Message}");
        }

        private string RemoveSpecialCharacters(string text)
        {
            return Regex.Replace(text, @"[^A-Za-z0-9.]", "");
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

        public void SendEffectVotingToGame()
        {
            if (effectVoting.IsEmpty())
            {
                return;
            }

            effectVoting.GetVotes(out string[] effects, out int[] votes);

            string voteString = $"votes:{effects[0]};{votes[0]};;{effects[1]};{votes[1]};;{effects[2]};{votes[2]};;{lastChoice}";
            ProcessHooker.SendPipeMessage(voteString);
        }

        private class EffectVoting
        {
            public readonly List<VotingElement> VotingElements;

            public EffectVoting()
            {
                VotingElements = new List<VotingElement>();
            }

            public bool IsEmpty()
            {
                return VotingElements.Count == 0;
            }

            public void Clear()
            {
                VotingElements.Clear();
            }

            public bool ContainsEffect(AbstractEffect effect)
            {
                return VotingElements.Any(e => e.Effect.GetDescription().Equals(effect.GetDescription()));
            }

            public void AddEffect(AbstractEffect effect)
            {
                VotingElements.Add(new VotingElement(VotingElements.Count, effect));
            }

            public void GetVotes(out string[] effects, out int[] votes)
            {
                VotingElement[] votingElements = VotingElements.ToArray();

                effects = new string[]
                {
                    votingElements[0].Effect.GetDescription(),
                    votingElements[1].Effect.GetDescription(),
                    votingElements[2].Effect.GetDescription()
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
                int possibleEffects = Math.Min(3, Config.Instance.TwitchAllowOnlyEnabledEffects ? EffectDatabase.EnabledEffects.Count : 3);
                while (VotingElements.Count != possibleEffects)
                {
                    AbstractEffect effect = EffectDatabase.GetRandomEffect(Config.Instance.TwitchAllowOnlyEnabledEffects);
                    if (!ContainsEffect(effect))
                    {
                        AddEffect(effect);
                    }
                }

                if (VotingElements.Count < 3)
                {
                    AbstractEffect effect = EffectDatabase.GetRandomEffect();
                    if (!ContainsEffect(effect))
                    {
                        AddEffect(effect);
                    }
                }
            }

            public VotingElement GetMajorityVote()
            {
                return VotingElements.OrderByDescending(e => e.Voters.Count).ToArray()[0];
            }

            public void TryAddVote(string username, int effectChoice)
            {
                if (!Config.Instance.TwitchAllowVoting)
                {
                    return;
                }

                VotingElements.ForEach(e => e.RemoveVoter(username));
                VotingElements[effectChoice].AddVoter(username);
            }

            public VotingElement GetRandomEffect(out string username, out int choice)
            {
                Random random = new Random();

                VotingElement element;
                List<VotingElement> newElements = VotingElements.FindAll(e => e.Voters.Count > 0);

                if (newElements.Count == 0)
                {
                    element = VotingElements.ToArray()[random.Next(VotingElements.Count)];
                }
                else
                {
                    element = newElements.ToArray()[random.Next(newElements.Count)];
                }

                username = element.GetRandomVoter();
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

            public string GetRandomVoter()
            {
                return Voters.Count == 0 ? "" : Voters.ToArray()[new Random().Next(Voters.Count)];
            }
        }
    }
}

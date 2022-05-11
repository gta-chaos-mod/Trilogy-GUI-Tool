// Copyright (c) 2019 Lordmau5
using GTAChaos.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace GTAChaos.Utils
{
    public class TwitchChatConnection : ITwitchConnection
    {
        public TwitchClient Client;
        private readonly TwitchAPI api;

        private readonly string AccessToken;
        private string Channel;
        private string Username;

        private readonly ChatEffectVoting effectVoting = new ChatEffectVoting();
        private readonly HashSet<string> rapidFireVoters = new HashSet<string>();
        private int VotingMode;

        private int lastChoice = -1;

        public TwitchChatConnection()
        {
            AccessToken = Config.Instance().TwitchAccessToken;

            if (AccessToken == null || AccessToken == "")
            {
                return;
            }

            api = new TwitchAPI();
            api.Settings.ClientId = "d9rifiqcfbgz93ft16o8bsya9ho2ih";
            api.Settings.AccessToken = AccessToken;
        }

        public TwitchClient GetTwitchClient()
        {
            return Client;
        }

        private void InitializeTwitchClient()
        {
            ConnectionCredentials credentials = new ConnectionCredentials(Username, AccessToken);

            WebSocketClient customClient = new WebSocketClient(
                new ClientOptions()
                {
                    MessagesAllowedInPeriod = 750,
                    ThrottlingPeriod = TimeSpan.FromSeconds(30)
                }
            );
            Client = new TwitchClient(customClient);
            Client.Initialize(credentials, Channel);

            Client.OnMessageReceived += Client_OnMessageReceived;
            Client.OnConnected += Client_OnConnected;

            Client.OnConnectionError += Client_OnConnectionError;
            Client.OnIncorrectLogin += Client_OnIncorrectLogin;
        }

        public async Task<bool> TryConnect()
        {
            Kill();

            var data = await api.Auth.ValidateAccessTokenAsync();
            if (data == null) return false;

            Username = data.Login;
            Channel = data.Login;

            InitializeTwitchClient();

            Client.Connect();

            return true;
        }

        public bool IsConnected()
        {
            return Client?.IsConnected == true;
        }

        private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            Kill();

            //Client?.Initialize(new ConnectionCredentials(Username, AccessToken), Channel);

            //Client?.Connect();
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            OnConnected?.Invoke(this, e);

            SendMessage("Connected!");
        }

        private void Client_OnIncorrectLogin(object sender, OnIncorrectLoginArgs e)
        {
            OnLoginError?.Invoke(sender, e);
        }

        public void Kill()
        {
            if (Client != null)
            {
                OnDisconnected?.Invoke(this, new EventArgs());

                Client.OnMessageReceived -= Client_OnMessageReceived;
                Client.OnConnected -= Client_OnConnected;

                Client.OnConnectionError -= Client_OnConnectionError;
                Client.OnIncorrectLogin -= Client_OnIncorrectLogin;

                Client.Disconnect();
            }

            Client = null;
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

                    if (Config.Instance().TwitchEnableRapidFire)
                    {
                        SendMessage($"Cooldown has started! ({untilRapidFire} until Rapid-Fire) - Enabled effects: {allEffects}");
                        if (untilRapidFire == 1)
                        {
                            SendMessage("Rapid-Fire is coming up! Get your cheats ready! - List of all effects: https://bit.ly/gta-sa-chaos-mod");
                        }
                    }
                    else
                    {
                        SendMessage($"Cooldown has started! - Enabled effects: {allEffects}");
                    }
                }
                else
                {
                    if (Config.Instance().TwitchEnableRapidFire)
                    {
                        SendMessage($"Cooldown has started! ({untilRapidFire} until Rapid-Fire)");
                        if (untilRapidFire == 1)
                        {
                            SendMessage("Rapid-Fire is coming up! Get your cheats ready! - List of all effects: https://bit.ly/gta-sa-chaos-mod");
                        }
                    }
                    else
                    {
                        SendMessage($"Cooldown has started!");
                    }

                }
            }
        }

        public List<IVotingElement> GetVotedEffects()
        {
            List<IVotingElement> elements = Config.Instance().TwitchMajorityVotes ? effectVoting.GetMajorityVotes() : effectVoting.GetTrulyRandomVotes();
            elements.ForEach(e => e.GetEffect().ResetTwitchVoter());

            lastChoice = elements.Count > 1 ? -1 : elements.First().GetId();

            return elements;
        }

        private void SendMessage(string message, bool prefix = true)
        {
            if (IsConnected() && Channel != null && message != null)
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

            public List<IVotingElement> GetTrulyRandomVotes()
            {
                List<IVotingElement> votes = new List<IVotingElement>();

                // Calculate total votes
                int totalVotes = 0;
                votingElements.ForEach(e => totalVotes += e.Voters.Count);

                if (totalVotes == 0)
                {
                    return votes;
                }

                /*
                 * 130 140 50 - total 320
                 * rN 180
                 * rN > 130 -> rN -= 130
                 * 180 - 130 = 50
                 * rN <= 140 -> Add, then break
                 * 
                 * 0 1 0 - total 1
                 * rN 1
                 * rN > 0 -> rN -= 0
                 * 1 - 0 = 1
                 * rN <= 1 -> Add, then break
                 * 
                 * 
                 * 40 0 20 - total 60
                 * rN 45
                 * rN > 40 -> rN -= 40
                 * 45 - 40 = 5
                 * rN > 0 -> rN -= 0
                 * 5 - 0 = 0
                 * rN <= 20 -> Add, then break
                 */

                // Calculate random number
                int randomNumber = new Random().Next(totalVotes) + 1;

                foreach (var e in votingElements)
                {
                    if (randomNumber > e.Voters.Count)
                    {
                        randomNumber -= e.Voters.Count;
                        continue;
                    }

                    votes.Add(e);
                    break;
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

        // Events
        public event EventHandler<EventArgs> OnConnected;
        public event EventHandler<EventArgs> OnDisconnected;
        public event EventHandler<EventArgs> OnLoginError;
        public event EventHandler<RapidFireEventArgs> OnRapidFireEffect;

        public virtual void RapidFireEffect(RapidFireEventArgs e)
        {
            OnRapidFireEffect?.Invoke(this, e);
        }
    }
}

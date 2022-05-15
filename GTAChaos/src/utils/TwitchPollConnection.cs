// Copyright (c) 2019 Lordmau5
using GTAChaos.Effects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Polls;
using TwitchLib.Api.Helix.Models.Polls.CreatePoll;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace GTAChaos.Utils
{
    public class TwitchPollConnection : IStreamConnection
    {
        public TwitchClient Client;
        private WebSocketClient customClient;
        private readonly TwitchAPI api;

        private readonly string AccessToken;
        private readonly string ClientID;
        private string Username;
        private string Channel;
        private string UserID;

        private readonly PollEffectVoting effectVoting = new PollEffectVoting();
        private readonly HashSet<string> rapidFireVoters = new HashSet<string>();
        private int VotingMode;

        private int lastChoice = -1;

        private bool createdPoll = false;
        private Poll activePoll;

        private System.Timers.Timer activePollTimer;

        public TwitchPollConnection()
        {
            AccessToken = Config.Instance().StreamAccessToken;
            ClientID = Config.Instance().StreamClientID;

            if (string.IsNullOrEmpty(AccessToken) || string.IsNullOrEmpty(ClientID)) return;

            api = new TwitchAPI();
            api.Settings.ClientId = ClientID;
            api.Settings.AccessToken = AccessToken;

            activePollTimer = new()
            {
                AutoReset = true,
                Interval = 1000
            };
            activePollTimer.Elapsed += ActivePollTimer_Elapsed;
            activePollTimer.Start();
        }

        private async void ActivePollTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!createdPoll || activePoll == null) return;

            List<string> pollIds = new List<string>();
            pollIds.Add(activePoll.Id);

            var polls = await api.Helix.Polls.GetPollsAsync(UserID, pollIds);
            var updatedPoll = polls.Data[0];

            activePoll = updatedPoll;
            for (int i = 0; i < 3; i++)
            {
                effectVoting.SetVotes(i, activePoll.Choices[i].Votes);
            }
        }

        private void InitializeTwitchClient()
        {
            ConnectionCredentials credentials = new ConnectionCredentials(Username, AccessToken);

            customClient = new WebSocketClient(
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

            var data = await api.Auth.ValidateAccessTokenAsync(AccessToken);
            if (data == null)
            {
                OnLoginError?.Invoke(this, new EventArgs());
                return false;
            }

            Username = data.Login;
            Channel = data.Login;
            UserID = data.UserId;

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
            //Kill();

            Client?.Reconnect();

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

                Client.Disconnect();
            }
            Client = null;

            customClient?.Dispose();
            customClient = null;
        }

        public int GetRemaining()
        {
            if (activePoll == null) return -1;
            if (activePoll.Status == "COMPLETED") return 0;
            return activePoll.DurationSeconds;
        }

        private async Task<bool> TryPoll(CreatePollRequest createPoll, int tries = 0)
        {
            try
            {
                activePoll = (await api.Helix.Polls.CreatePollAsync(createPoll, AccessToken)).Data[0];
            }
            catch
            {
                tries += 1;
                Debug.WriteLine($"Couldn't start poll. Attempt {tries}/10.");

                if (tries > 5)
                {
                    return false;
                }

                await Task.Delay(500);

                return await TryPoll(createPoll, tries);
            }

            return true;
        }

        public async void SetVoting(int votingMode, int untilRapidFire = -1, List<IVotingElement> votingElements = null)
        {
            VotingMode = votingMode;
            if (VotingMode == 1)
            {
                effectVoting.Clear();
                effectVoting.GenerateRandomEffects();
                lastChoice = -1;

                if (Config.Instance().TwitchPollsPostMessages)
                {
                    if (Config.Instance().StreamCombineChatMessages)
                    {
                        string messageToSend = "Voting has started! ";

                        foreach (PollVotingElement element in effectVoting.GetVotingElements())
                        {
                            string description = element.Effect.GetDisplayName();
                            messageToSend += $"#{element.Id + 1}: {description}. ";
                        }

                        SendMessage(messageToSend);
                    }
                    else
                    {
                        SendMessage("Voting has started!");

                        foreach (PollVotingElement element in effectVoting.GetVotingElements())
                        {
                            string description = element.Effect.GetDisplayName();
                            SendMessage($"#{element.Id + 1}: {description}");
                        }
                    }
                }

                CreatePollRequest createPoll = new CreatePollRequest()
                {
                    Title = "[GTA Chaos] Next Effect",
                    BroadcasterId = UserID,
                    DurationSeconds = Config.Instance().StreamVotingTime / 1000,
                    Choices = effectVoting.GetPollChoices(),
                    BitsVotingEnabled = Config.Instance().TwitchPollsBitsCost > 0,
                    BitsPerVote = Config.Instance().TwitchPollsBitsCost,
                    ChannelPointsVotingEnabled = Config.Instance().TwitchPollsChannelPointsCost > 0,
                    ChannelPointsPerVote = Config.Instance().TwitchPollsChannelPointsCost,
                };

                List<string> effects = new List<string>();
                foreach (var el in createPoll.Choices)
                {
                    effects.Add(el.Title);
                }
                Debug.WriteLine(string.Join(", ", effects.ToArray()));

                createdPoll = await TryPoll(createPoll);

                //SocketBroadcast(JsonConvert.SerializeObject(createPoll));
            }
            else if (VotingMode == 2)
            {
                rapidFireVoters.Clear();
                SendMessage("ATTENTION, ALL GAMERS! RAPID-FIRE HAS BEGUN! VALID EFFECTS WILL BE ENABLED FOR 15 SECONDS!");
            }
            else if (VotingMode == 3) // Poll Failed
            {
                SendEffectVotingToGame(false);

                if (Config.Instance().StreamEnableRapidFire)
                {
                    SendMessage($"Cooldown has started! ({untilRapidFire} until Rapid-Fire) - Poll Failed :(");

                    if (untilRapidFire == 1)
                    {
                        SendMessage("Rapid-Fire is coming up! Get your cheats ready! - List of all effects: https://bit.ly/gta-sa-chaos-mod");
                    }
                }
                else
                {
                    SendMessage($"Cooldown has started! - Poll Failed :(");
                }

                // Make sure we end poll, thank
                //if (activePoll != null)
                //{
                //    var response = api.Helix.Polls.EndPollAsync(UserID, activePoll.Id, TwitchLib.Api.Core.Enums.PollStatusEnum.ARCHIVED);
                //}
                activePoll = null;
                createdPoll = false;
            }
            else
            {
                if (votingElements != null && votingElements.Count > 0)
                {
                    SendEffectVotingToGame(false);

                    string allEffects = string.Join(", ", votingElements.Select(e => e.GetEffect().GetDisplayName()));

                    if (Config.Instance().StreamEnableRapidFire)
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

                    // Make sure we end poll, thank
                    //if (activePoll != null)
                    //{
                    //    var response = api.Helix.Polls.EndPollAsync(UserID, activePoll.Id, TwitchLib.Api.Core.Enums.PollStatusEnum.ARCHIVED);
                    //}
                    activePoll = null;
                    createdPoll = false;
                }
                else
                {
                    if (Config.Instance().StreamEnableRapidFire)
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
            List<IVotingElement> elements = Config.Instance().StreamMajorityVotes ? effectVoting.GetMajorityVotes() : effectVoting.GetTrulyRandomVotes();
            foreach (var e in elements)
            {
                e.GetEffect().ResetStreamVoter();
            }

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

            // Rapid Fire
            if (VotingMode == 2)
            {
                if (rapidFireVoters.Contains(username))
                {
                    return;
                }

                AbstractEffect effect = EffectDatabase.GetByWord(message, Config.Instance().StreamAllowOnlyEnabledEffectsRapidFire);
                if (effect == null || !effect.IsRapidFire())
                {
                    return;
                }

                RapidFireEffect(new RapidFireEventArgs()
                {
                    Effect = effect.SetTreamVoter(username)
                });

                rapidFireVoters.Add(username);

                return;
            }
        }

        private string RemoveSpecialCharacters(string text)
        {
            return Regex.Replace(text, @"[^A-Za-z0-9]", "");
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

        private class PollEffectVoting
        {
            private readonly List<PollVotingElement> votingElements;

            public PollEffectVoting()
            {
                votingElements = new List<PollVotingElement>();
            }

            public bool IsEmpty()
            {
                return votingElements.Count == 0;
            }

            public void Clear()
            {
                votingElements.Clear();
            }

            public List<PollVotingElement> GetVotingElements()
            {
                return votingElements;
            }

            public TwitchLib.Api.Helix.Models.Polls.CreatePoll.Choice[] GetPollChoices()
            {
                var choices = new List<TwitchLib.Api.Helix.Models.Polls.CreatePoll.Choice>();

                foreach (var element in GetVotingElements())
                {
                    string description = element.Effect.GetDisplayName();

                    choices.Add(new TwitchLib.Api.Helix.Models.Polls.CreatePoll.Choice()
                    {
                        Title = description.Substring(0, Math.Min(25, description.Length))
                    });
                }

                return choices.ToArray();
            }

            public bool ContainsEffect(AbstractEffect effect)
            {
                return votingElements.Any(e => e.Effect.GetDisplayName().Equals(effect.GetDisplayName()));
            }

            public void AddEffect(AbstractEffect effect)
            {
                votingElements.Add(new PollVotingElement(votingElements.Count, effect));
            }

            public void GetVotes(out string[] effects, out int[] votes, bool undetermined = false)
            {
                PollVotingElement[] votingElements = GetVotingElements().ToArray();

                effects = new string[]
                {
                    undetermined ? "???" : votingElements[0].Effect.GetDisplayName(),
                    undetermined ? "???" : votingElements[1].Effect.GetDisplayName(),
                    undetermined ? "???" : votingElements[2].Effect.GetDisplayName()
                };

                votes = new int[]
                {
                    votingElements[0].Votes,
                    votingElements[1].Votes,
                    votingElements[2].Votes
                };
            }

            public void SetVotes(int elementId, int votes)
            {
                if (elementId < 0 || elementId > 2) return;

                if (votingElements.Count > elementId) votingElements[elementId].Votes = votes;
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
                PollVotingElement[] elements = votingElements.OrderByDescending(e =>
                {
                    if (e.Votes > maxVotes)
                    {
                        maxVotes = e.Votes;
                    }
                    return e.Votes;
                }).Where(e => e.Votes == maxVotes).ToArray();

                List<IVotingElement> votes = new List<IVotingElement>();
                if (Config.Instance().StreamEnableMultipleEffects)
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
                foreach (var e in votingElements)
                {
                    totalVotes += e.Votes;
                }

                if (totalVotes == 0)
                {
                    // TODO: Random effect on 0 votes option
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
                    if (randomNumber > e.Votes)
                    {
                        randomNumber -= e.Votes;
                        continue;
                    }

                    votes.Add(e);
                    break;
                }
                return votes;
            }
        }

        public class PollVotingElement : IVotingElement
        {
            public int Id { get; set; }

            public AbstractEffect Effect { get; set; }

            public int Votes { get; set; }

            public PollVotingElement(int id, AbstractEffect effect)
            {
                Id = id;
                Effect = effect;
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
                return Votes;
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

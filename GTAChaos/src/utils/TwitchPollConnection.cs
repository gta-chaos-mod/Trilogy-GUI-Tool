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

        private readonly PollEffectVoting effectVoting = new();
        private readonly HashSet<string> rapidFireVoters = new();
        private Shared.VOTING_MODE VotingMode;

        private int lastChoice = -1;

        private bool createdPoll = false;
        private Poll activePoll;

        private readonly System.Timers.Timer activePollTimer;

        public TwitchPollConnection()
        {
            this.AccessToken = Config.Instance().StreamAccessToken;
            this.ClientID = Config.Instance().StreamClientID;

            if (string.IsNullOrEmpty(this.AccessToken) || string.IsNullOrEmpty(this.ClientID))
            {
                return;
            }

            this.api = new TwitchAPI();
            this.api.Settings.ClientId = this.ClientID;
            this.api.Settings.AccessToken = this.AccessToken;

            this.activePollTimer = new()
            {
                AutoReset = true,
                Interval = 1000
            };
            this.activePollTimer.Elapsed += this.ActivePollTimer_Elapsed;
            this.activePollTimer.Start();
        }

        private async void ActivePollTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!this.createdPoll || this.activePoll == null)
            {
                return;
            }

            List<string> pollIds = new()
            {
                this.activePoll.Id
            };

            TwitchLib.Api.Helix.Models.Polls.GetPolls.GetPollsResponse polls = await this.api.Helix.Polls.GetPollsAsync(this.UserID, pollIds);
            Poll updatedPoll = polls.Data[0];

            this.activePoll = updatedPoll;
            for (int i = 0; i < 3; i++)
            {
                this.effectVoting.SetVotes(i, this.activePoll.Choices[i].Votes);
            }
        }

        private void InitializeTwitchClient()
        {
            ConnectionCredentials credentials = new(this.Username, this.AccessToken);

            this.customClient = new WebSocketClient(
                new ClientOptions()
                {
                    MessagesAllowedInPeriod = 750,
                    ThrottlingPeriod = TimeSpan.FromSeconds(30)
                }
            );
            this.Client = new TwitchClient(this.customClient);
            this.Client.Initialize(credentials, this.Channel);

            this.Client.OnMessageReceived += this.Client_OnMessageReceived;
            this.Client.OnConnected += this.Client_OnConnected;

            this.Client.OnConnectionError += this.Client_OnConnectionError;
            this.Client.OnIncorrectLogin += this.Client_OnIncorrectLogin;
        }

        public async Task<bool> TryConnect()
        {
            this.Kill();

            TwitchLib.Api.Auth.ValidateAccessTokenResponse data = await this.api.Auth.ValidateAccessTokenAsync(this.AccessToken);
            if (data == null)
            {
                OnLoginError?.Invoke(this, new EventArgs());
                return false;
            }

            this.Username = data.Login;
            this.Channel = data.Login;
            this.UserID = data.UserId;

            this.InitializeTwitchClient();

            this.Client.Connect();

            return true;
        }

        public bool IsConnected() => this.Client?.IsConnected == true;

        private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e) =>
            //Kill();

            this.Client?.Reconnect();//Client?.Initialize(new ConnectionCredentials(Username, AccessToken), Channel);//Client?.Connect();

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            OnConnected?.Invoke(this, e);

            this.SendMessage("Connected!");
        }

        private void Client_OnIncorrectLogin(object sender, OnIncorrectLoginArgs e) => OnLoginError?.Invoke(sender, e);

        public void Kill()
        {
            if (this.Client != null)
            {
                OnDisconnected?.Invoke(this, new EventArgs());

                this.Client.Disconnect();
            }

            this.Client = null;

            this.customClient?.Dispose();
            this.customClient = null;
        }

        public int GetRemaining()
        {
            if (this.activePoll == null)
            {
                return -1;
            }

            return this.activePoll.Status == "COMPLETED" ? 0 : this.activePoll.DurationSeconds;
        }

        private async Task<bool> TryPoll(CreatePollRequest createPoll, int tries = 0)
        {
            try
            {
                this.activePoll = (await this.api.Helix.Polls.CreatePollAsync(createPoll, this.AccessToken)).Data[0];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());

                tries += 1;
                Debug.WriteLine($"Couldn't start poll. Attempt {tries}/10.");

                if (tries > 5)
                {
                    return false;
                }

                await Task.Delay(500);

                return await this.TryPoll(createPoll, tries);
            }

            return true;
        }

        public async void SetVoting(Shared.VOTING_MODE votingMode, int untilRapidFire = -1, List<IVotingElement> votingElements = null)
        {
            this.VotingMode = votingMode;
            if (this.VotingMode == Shared.VOTING_MODE.VOTING)
            {
                this.effectVoting.Clear();
                this.effectVoting.GenerateRandomEffects();
                this.lastChoice = -1;

                if (Config.Instance().TwitchPollsPostMessages)
                {
                    if (Config.Instance().StreamCombineChatMessages)
                    {
                        string messageToSend = "Voting has started! ";

                        foreach (PollVotingElement element in this.effectVoting.GetVotingElements())
                        {
                            string description = element.Effect.GetDisplayName(DisplayNameType.STREAM);
                            messageToSend += $"#{element.Id + 1}: {description}. ";
                        }

                        this.SendMessage(messageToSend);
                    }
                    else
                    {
                        this.SendMessage("Voting has started!");

                        foreach (PollVotingElement element in this.effectVoting.GetVotingElements())
                        {
                            string description = element.Effect.GetDisplayName(DisplayNameType.STREAM);
                            this.SendMessage($"#{element.Id + 1}: {description}");
                        }
                    }
                }

                CreatePollRequest createPoll = new()
                {
                    Title = "[GTA Chaos] Next Effect",
                    BroadcasterId = UserID,
                    DurationSeconds = Config.Instance().StreamVotingTime / 1000,
                    Choices = this.effectVoting.GetPollChoices(),
                    BitsVotingEnabled = Config.Instance().TwitchPollsBitsCost > 0,
                    BitsPerVote = Config.Instance().TwitchPollsBitsCost,
                    ChannelPointsVotingEnabled = Config.Instance().TwitchPollsChannelPointsCost > 0,
                    ChannelPointsPerVote = Config.Instance().TwitchPollsChannelPointsCost,
                };

                List<string> effects = new();
                foreach (TwitchLib.Api.Helix.Models.Polls.CreatePoll.Choice el in createPoll.Choices)
                {
                    effects.Add(el.Title);
                }

                Debug.WriteLine(string.Join(", ", effects.ToArray()));

                this.createdPoll = await this.TryPoll(createPoll);

                //SocketBroadcast(JsonConvert.SerializeObject(createPoll));
            }
            else if (this.VotingMode == Shared.VOTING_MODE.RAPID_FIRE)
            {
                this.rapidFireVoters.Clear();
                this.SendMessage("ATTENTION, ALL GAMERS! RAPID-FIRE HAS BEGUN! VALID EFFECTS WILL BE ENABLED FOR 15 SECONDS!");
            }
            else if (this.VotingMode == Shared.VOTING_MODE.ERROR) // Poll Failed
            {
                this.SendEffectVotingToGame(false);

                if (Config.Instance().StreamEnableRapidFire)
                {
                    this.SendMessage($"Cooldown has started! ({untilRapidFire} until Rapid-Fire) - Poll Failed :(");

                    if (untilRapidFire == 1)
                    {
                        this.SendMessage("Rapid-Fire is coming up! Get your cheats ready! - List of all effects: https://bit.ly/gta-sa-chaos-mod");
                    }
                }
                else
                {
                    this.SendMessage($"Cooldown has started! - Poll Failed :(");
                }

                // Make sure we end poll, thank
                //if (activePoll != null)
                //{
                //    var response = api.Helix.Polls.EndPollAsync(UserID, activePoll.Id, TwitchLib.Api.Core.Enums.PollStatusEnum.ARCHIVED);
                //}
                this.activePoll = null;
                this.createdPoll = false;
            }
            else
            {
                if (votingElements != null && votingElements.Count > 0)
                {
                    this.SendEffectVotingToGame(false);

                    string allEffects = string.Join(", ", votingElements.Select(e => e.GetEffect().GetDisplayName(DisplayNameType.STREAM)));

                    if (Config.Instance().StreamEnableRapidFire)
                    {
                        this.SendMessage($"Cooldown has started! ({untilRapidFire} until Rapid-Fire) - Enabled effects: {allEffects}");

                        if (untilRapidFire == 1)
                        {
                            this.SendMessage("Rapid-Fire is coming up! Get your cheats ready! - List of all effects: https://bit.ly/gta-sa-chaos-mod");
                        }
                    }
                    else
                    {
                        this.SendMessage($"Cooldown has started! - Enabled effects: {allEffects}");
                    }

                    // Make sure we end poll, thank
                    //if (activePoll != null)
                    //{
                    //    var response = api.Helix.Polls.EndPollAsync(UserID, activePoll.Id, TwitchLib.Api.Core.Enums.PollStatusEnum.ARCHIVED);
                    //}
                    this.activePoll = null;
                    this.createdPoll = false;
                }
                else
                {
                    if (Config.Instance().StreamEnableRapidFire)
                    {
                        this.SendMessage($"Cooldown has started! ({untilRapidFire} until Rapid-Fire)");

                        if (untilRapidFire == 1)
                        {
                            this.SendMessage("Rapid-Fire is coming up! Get your cheats ready! - List of all effects: https://bit.ly/gta-sa-chaos-mod");
                        }
                    }
                    else
                    {
                        this.SendMessage($"Cooldown has started!");
                    }
                }
            }
        }

        public List<IVotingElement> GetVotedEffects()
        {
            List<IVotingElement> elements = Config.Instance().StreamMajorityVotes ? this.effectVoting.GetMajorityVotes() : this.effectVoting.GetTrulyRandomVotes();
            foreach (IVotingElement e in elements)
            {
                e.GetEffect().SetSubtext($"{e.GetPercentage()}%");
            }

            this.lastChoice = elements.Count > 1 ? -1 : elements.First().GetId();

            return elements;
        }

        private void SendMessage(string message, bool prefix = true)
        {
            if (this.IsConnected() && this.Channel != null && message != null)
            {
                if (!this.Client.IsConnected)
                {
                    this.Client.Connect();
                    return;
                }

                if (this.Client.JoinedChannels.Count == 0)
                {
                    this.Client.JoinChannel(this.Channel);
                    return;
                }

                this.Client.SendMessage(this.Channel, $"{(prefix ? "[GTA Chaos] " : "")}{message}");
            }
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            string username = e.ChatMessage.Username;
            string message = this.RemoveSpecialCharacters(e.ChatMessage.Message);

            // Rapid Fire
            if (this.VotingMode == Shared.VOTING_MODE.RAPID_FIRE)
            {
                if (this.rapidFireVoters.Contains(username))
                {
                    return;
                }

                AbstractEffect effect = EffectDatabase.GetByWord(message, Config.Instance().StreamAllowOnlyEnabledEffectsRapidFire);
                if (effect == null || !effect.IsRapidFire())
                {
                    return;
                }

                this.RapidFireEffect(new RapidFireEventArgs()
                {
                    Effect = effect.SetSubtext(username)
                });

                this.rapidFireVoters.Add(username);

                return;
            }
        }

        private string RemoveSpecialCharacters(string text) => Regex.Replace(text, @"[^A-Za-z0-9]", "");

        public void SendEffectVotingToGame(bool undetermined = true)
        {
            if (this.effectVoting.IsEmpty())
            {
                return;
            }

            this.effectVoting.GetVotes(out string[] effects, out int[] votes, undetermined);

            if (Shared.Multiplayer != null)
            {
                Shared.Multiplayer.SendVotes(effects, votes, this.lastChoice, !undetermined);
            }
            else
            {
                WebsocketHandler.INSTANCE.SendVotes(effects, votes, this.lastChoice);
            }
        }

        private class PollEffectVoting
        {
            private readonly List<PollVotingElement> votingElements = new();

            public bool IsEmpty() => this.votingElements.Count == 0;

            public void Clear() => this.votingElements.Clear();

            public List<PollVotingElement> GetVotingElements() => this.votingElements;

            public int GetTotalVotes()
            {
                int votes = 0;
                foreach (PollVotingElement element in this.votingElements)
                {
                    votes += element.Votes;
                }

                return votes;
            }

            public TwitchLib.Api.Helix.Models.Polls.CreatePoll.Choice[] GetPollChoices()
            {
                List<TwitchLib.Api.Helix.Models.Polls.CreatePoll.Choice> choices = new();

                foreach (PollVotingElement element in this.GetVotingElements())
                {
                    string description = element.Effect.GetDisplayName(DisplayNameType.STREAM);

                    choices.Add(new TwitchLib.Api.Helix.Models.Polls.CreatePoll.Choice()
                    {
                        Title = description.Substring(0, Math.Min(25, description.Length))
                    });
                }

                return choices.ToArray();
            }

            public bool ContainsEffect(AbstractEffect effect) => this.votingElements.Any(e => e.Effect.GetDisplayName(DisplayNameType.STREAM).Equals(effect.GetDisplayName(DisplayNameType.STREAM)));

            public void AddEffect(AbstractEffect effect) => this.votingElements.Add(new PollVotingElement(this.votingElements.Count, effect));

            public void GetVotes(out string[] effects, out int[] votes, bool undetermined = false)
            {
                PollVotingElement[] votingElements = this.GetVotingElements().ToArray();

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
                if (elementId is < 0 or > 2)
                {
                    return;
                }

                if (this.votingElements.Count > elementId)
                {
                    this.votingElements[elementId].Votes = votes;
                }

                foreach (PollVotingElement element in this.votingElements)
                {
                    element.Percentage = (int)Math.Round((double)element.Votes / this.GetTotalVotes() * 100);
                }
            }

            public void GenerateRandomEffects()
            {
                int possibleEffects = Math.Min(3, EffectDatabase.EnabledEffects.Count);
                while (this.votingElements.Count != possibleEffects)
                {
                    AbstractEffect effect = EffectDatabase.GetRandomEffect(true);
                    if (effect.IsTwitchEnabled() && !this.ContainsEffect(effect))
                    {
                        this.AddEffect(effect);
                    }
                }

                while (this.votingElements.Count < 3)
                {
                    AbstractEffect effect = EffectDatabase.GetRandomEffect();
                    if (effect.IsTwitchEnabled() && !this.ContainsEffect(effect))
                    {
                        this.AddEffect(effect);
                    }
                }
            }

            public List<IVotingElement> GetMajorityVotes()
            {
                // If there are effects that have the same amount of votes, get a random one
                int maxVotes = 0;
                PollVotingElement[] elements = this.votingElements.OrderByDescending(e =>
                {
                    if (e.Votes > maxVotes)
                    {
                        maxVotes = e.Votes;
                    }

                    return e.Votes;
                }).Where(e => e.Votes == maxVotes).ToArray();

                List<IVotingElement> votes = new();
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
                List<IVotingElement> votes = new();

                // Calculate total votes
                int totalVotes = 0;
                foreach (PollVotingElement e in this.votingElements)
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

                foreach (PollVotingElement e in this.votingElements)
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

            public int Percentage { get; set; }

            public PollVotingElement(int id, AbstractEffect effect)
            {
                this.Id = id;
                this.Effect = effect;
            }

            public int GetId() => this.Id;

            public AbstractEffect GetEffect() => this.Effect;

            public int GetVotes() => this.Votes;

            public int GetPercentage() => this.Percentage;
        }

        // Events
        public event EventHandler<EventArgs> OnConnected;
        public event EventHandler<EventArgs> OnDisconnected;
        public event EventHandler<EventArgs> OnLoginError;
        public event EventHandler<RapidFireEventArgs> OnRapidFireEffect;

        public virtual void RapidFireEffect(RapidFireEventArgs e) => OnRapidFireEffect?.Invoke(this, e);
    }
}

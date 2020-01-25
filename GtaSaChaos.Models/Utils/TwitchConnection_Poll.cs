// Copyright (c) 2019 Lordmau5
using Fleck;
using GtaChaos.Models.Effects;
using GtaChaos.Models.Effects.@abstract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace GtaChaos.Models.Utils
{
    public class Auth
    {
        public string type = "auth";
        public string data;
    }

    public class CreatePoll
    {
        public string type = "create";
        public string title = "[GTA Chaos] Next Effect";
        public int duration;
        public ICollection<string> choices;
        public bool subscriberMultiplier;
        public bool subscriberOnly;
        public int bits;
    }

    public class EndPoll
    {
        public string type = "end";
        public string id;
    }

    public class PollType
    {
        public string type;
    }

    public class PollChoice
    {
        public string text;
        public int votes;
    }

    public class Poll
    {
        public string id;
        public bool ended;
        public int duration;
        public int remaining;
        public PollChoice[] choices;
    }

    public class PollCreateData
    {
        public string type;
        public string id;
    }

    public class PollUpdateData
    {
        public string type;
        public Poll poll;
    }

    public class TwitchConnection_Poll : ITwitchConnection
    {
        public TwitchClient Client;

        private readonly string Channel;
        private readonly string Username;
        private readonly string Oauth;

        private readonly PollEffectVoting effectVoting = new PollEffectVoting();
        private readonly HashSet<string> rapidFireVoters = new HashSet<string>();
        private int VotingMode;

        private int lastChoice = -1;

        private bool createdPoll = false;

        private Poll activePoll = null;
        private readonly WebSocketServer socket = null;
        private readonly List<IWebSocketConnection> connections = new List<IWebSocketConnection>();

        public TwitchConnection_Poll()
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
            if (Environment.OSVersion.Version.Major < 10)
            {
                protocol = TwitchLib.Client.Enums.ClientProtocol.TCP;
            }

            TryConnect(credentials, protocol);

            socket = new WebSocketServer("ws://0.0.0.0:31337");
            socket.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Auth auth = new Auth
                    {
                        data = Config.Instance().TwitchPollsPassphrase
                    };
                    socket.Send(JsonConvert.SerializeObject(auth));

                    connections.Add(socket);
                };

                socket.OnMessage = message =>
                {
                    PollType pollType = JsonConvert.DeserializeObject<PollType>(message);
                    if (pollType.type == "created")
                    {
                        PollCreateData data = JsonConvert.DeserializeObject<PollCreateData>(message);

                        if (createdPoll)
                        {
                            activePoll = new Poll()
                            {
                                id = data.id,
                                remaining = Config.Instance().TwitchVotingTime
                            };
                            createdPoll = false;
                        }
                    }
                    else if (pollType.type == "update")
                    {
                        PollUpdateData data = JsonConvert.DeserializeObject<PollUpdateData>(message);

                        if (activePoll == null || activePoll.id != data.poll.id)
                        {
                            return;
                        }

                        activePoll = data.poll;

                        for (int i = 0; i < 3; i++)
                        {
                            effectVoting.SetVotes(i, data.poll.choices[i].votes);
                        }
                    }
                };

                socket.OnClose = () =>
                {
                    if (connections.Contains(socket))
                    {
                        connections.Remove(socket);
                    }
                };
            });
        }

        public TwitchClient GetTwitchClient()
        {
            return Client;
        }

        private void SocketBroadcast(string message)
        {
            if (socket != null)
            {
                connections.RemoveAll(con => !con.IsAvailable);
                if (connections.Count > 0)
                {
                    connections[0].Send(message);
                }
            }
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
            if (activePoll == null) return -1;
            if (activePoll.ended) return 0;
            return activePoll.remaining;
        }

        public void SetVoting(int votingMode, int untilRapidFire = -1, List<IVotingElement> votingElements = null)
        {
            VotingMode = votingMode;
            if (VotingMode == 1)
            {
                effectVoting.Clear();
                effectVoting.GenerateRandomEffects();
                lastChoice = -1;

                if (Config.Instance().TwitchPollsPostMessages)
                {
                    if (Config.Instance().TwitchCombineChatMessages)
                    {
                        string messageToSend = "Voting has started! On mobile? Type \"/vote ID\" (1, 2 or 3) to vote instead! ";

                        foreach (PollVotingElement element in effectVoting.GetVotingElements())
                        {
                            string description = element.Effect.GetDescription();
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
                        SendMessage("Voting has started! On mobile? Type \"/vote ID\" (1, 2 or 3) to vote instead!");

                        foreach (PollVotingElement element in effectVoting.GetVotingElements())
                        {
                            string description = element.Effect.GetDescription();
                            if (element.Effect.Word.Equals("IWontTakeAFreePass") && Config.Instance().TwitchAppendFakePassCurrentMission)
                            {
                                description = $"{description} (Fake)";
                            }

                            SendMessage($"#{element.Id + 1}: {description}");
                        }
                    }
                }

                CreatePoll createPoll = new CreatePoll()
                {
                    duration = Config.Instance().TwitchVotingTime / 1000,
                    choices = effectVoting.GetVotingElements().Select(elements =>
                    {
                        string description = elements.Effect.GetDescription();
                        if (elements.Effect.Word.Equals("IWontTakeAFreePass") && Config.Instance().TwitchAppendFakePassCurrentMission)
                        {
                            description = $"{description} (Fake)";
                        }
                        else if (elements.Effect.Word.Equals("HONKHONK"))
                        {
                            // Twitch doesn't like honks, so Honk Honk, everyone!
                            description = description.Replace("O", "ഠ");
                        }
                        return description;
                    }).ToList(),
                    subscriberMultiplier = Config.Instance().TwitchPollsSubcriberMultiplier,
                    subscriberOnly = Config.Instance().TwitchPollsSubscriberOnly,
                    bits = Config.Instance().TwitchPollsBitsCost
                };

                createdPoll = true;

                SocketBroadcast(JsonConvert.SerializeObject(createPoll));
            }
            else if (VotingMode == 2)
            {
                rapidFireVoters.Clear();
                SendMessage("ATTENTION, ALL GAMERS! RAPID-FIRE HAS BEGUN! VALID EFFECTS WILL BE ENABLED FOR 15 SECONDS!");
            }
            else if (VotingMode == 3) // Poll Failed
            {
                SendEffectVotingToGame(false);

                SendMessage($"Cooldown has started! ({untilRapidFire} until Rapid-Fire) - Poll Failed :(");

                if (untilRapidFire == 1)
                {
                    SendMessage("Rapid-Fire is coming up! Get your cheats ready! - List of all effects: https://bit.ly/gta-sa-chaos-mod");
                }

                // Make sure we end poll, thank
                //if (activePoll != null)
                //{
                //    EndPoll endPoll = new EndPoll()
                //    {
                //        id = activePoll.id
                //    };

                //    SocketBroadcast(JsonConvert.SerializeObject(endPoll));
                //}
                activePoll = null;
            }
            else
            {
                if (votingElements != null && votingElements.Count > 0)
                {
                    SendEffectVotingToGame(false);

                    string allEffects = string.Join(", ", votingElements.Select(e => e.GetEffect().GetDescription()));

                    SendMessage($"Cooldown has started! ({untilRapidFire} until Rapid-Fire) - Enabled effects: {allEffects}");

                    if (untilRapidFire == 1)
                    {
                        SendMessage("Rapid-Fire is coming up! Get your cheats ready! - List of all effects: https://bit.ly/gta-sa-chaos-mod");
                    }

                    // Make sure we end poll, thank
                    //if (activePoll != null)
                    //{
                    //    EndPoll endPoll = new EndPoll()
                    //    {
                    //        id = activePoll.id
                    //    };

                    //    SocketBroadcast(JsonConvert.SerializeObject(endPoll));
                    //}
                    activePoll = null;
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

        public List<IVotingElement> GetMajorityVotes()
        {
            List<IVotingElement> elements = effectVoting.GetMajorityVotes();
            elements.ForEach(e => e.GetEffect().ResetVoter());

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

            // Rapid Fire
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
                string voteString = $"votes:{effects[0]};{votes[0]};;{effects[1]};{votes[1]};;{effects[2]};{votes[2]};;{lastChoice}";
                ProcessHooker.SendPipeMessage(voteString);
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

            public bool ContainsEffect(AbstractEffect effect)
            {
                return votingElements.Any(e => e.Effect.GetDescription().Equals(effect.GetDescription()));
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
                    undetermined ? "???" : votingElements[0].Effect.GetDescription(),
                    undetermined ? "???" : votingElements[1].Effect.GetDescription(),
                    undetermined ? "???" : votingElements[2].Effect.GetDescription()
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
        }

        public event EventHandler<RapidFireEventArgs> OnRapidFireEffect;

        public virtual void RapidFireEffect(RapidFireEventArgs e)
        {
            OnRapidFireEffect?.Invoke(this, e);
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
        }
    }
}

// Copyright (c) 2019 Lordmau5
using GTAChaos.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace GTAChaos.Utils
{
    public class DebugConnection : IStreamConnection
    {
        private readonly ChatEffectVoting effectVoting = new();
        private readonly HashSet<string> rapidFireVoters = new();
        private Shared.VOTING_MODE VotingMode;

        private VoteChoice lastChoice = VoteChoice.UNDETERMINED;

        public int GetRemaining() => 0;

        public async Task<bool> TryConnect()
        {
            await Task.Run(() => OnConnected?.Invoke(this, new EventArgs()));
            return true;
        }

        public bool IsConnected() => true;

        public void Kill()
        {
        }

        public async void SetVoting(Shared.VOTING_MODE votingMode, int untilRapidFire = -1, List<IVotingElement> votingElements = null)
        {
            await Task.Run(() =>
            {
                this.VotingMode = votingMode;
                if (this.VotingMode == Shared.VOTING_MODE.VOTING)
                {
                    this.effectVoting.Clear();
                    this.effectVoting.GenerateRandomEffects();
                    //this.effectVoting?.TryAddVote("memes", 0);
                    this.lastChoice = VoteChoice.UNDETERMINED;
                }
                else if (this.VotingMode == Shared.VOTING_MODE.COOLDOWN)
                {
                    this.SendEffectVotingToGame(false);
                }

                //return;

                //this.VotingMode = votingMode;
                //if (this.VotingMode == Shared.VOTING_MODE.VOTING)
                //{
                //    this.effectVoting.Clear();
                //    this.effectVoting.GenerateRandomEffects();
                //    this.lastChoice = -1;

                //    if (Config.Instance().TwitchPollsPostMessages)
                //    {
                //        if (Config.Instance().StreamCombineChatMessages)
                //        {
                //            string messageToSend = "Voting has started! ";

                //            foreach (PollVotingElement element in this.effectVoting.GetVotingElements())
                //            {
                //                string description = element.Effect.GetDisplayName(DisplayNameType.STREAM);
                //                messageToSend += $"#{element.Id + 1}: {description}. ";
                //            }

                //            this.SendMessage(messageToSend);
                //        }
                //        else
                //        {
                //            this.SendMessage("Voting has started!");

                //            foreach (PollVotingElement element in this.effectVoting.GetVotingElements())
                //            {
                //                string description = element.Effect.GetDisplayName(DisplayNameType.STREAM);
                //                this.SendMessage($"#{element.Id + 1}: {description}");
                //            }
                //        }
                //    }

                //    //CreatePollRequest createPoll = new()
                //    //{
                //    //    Title = "[GTA Chaos] Next Effect",
                //    //    BroadcasterId = UserID,
                //    //    DurationSeconds = Config.Instance().StreamVotingTime / 1000,
                //    //    Choices = this.effectVoting.GetPollChoices(),
                //    //    BitsVotingEnabled = Config.Instance().TwitchPollsBitsCost > 0,
                //    //    BitsPerVote = Config.Instance().TwitchPollsBitsCost,
                //    //    ChannelPointsVotingEnabled = Config.Instance().TwitchPollsChannelPointsCost > 0,
                //    //    ChannelPointsPerVote = Config.Instance().TwitchPollsChannelPointsCost,
                //    //};

                //    //List<string> effects = new();
                //    //foreach (TwitchLib.Api.Helix.Models.Polls.CreatePoll.Choice el in createPoll.Choices)
                //    //{
                //    //    effects.Add(el.Title);
                //    //}

                //    //Debug.WriteLine(string.Join(", ", effects.ToArray()));

                //    //this.createdPoll = await this.TryPoll(createPoll);

                //    //SocketBroadcast(JsonConvert.SerializeObject(createPoll));
                //}
                //else if (this.VotingMode == Shared.VOTING_MODE.RAPID_FIRE)
                //{
                //    this.rapidFireVoters.Clear();
                //    //this.SendMessage("ATTENTION, ALL GAMERS! RAPID-FIRE HAS BEGUN! VALID EFFECTS WILL BE ENABLED FOR 15 SECONDS!");
                //}
                //else if (this.VotingMode == Shared.VOTING_MODE.ERROR) // Poll Failed
                //{
                //    this.SendEffectVotingToGame(false);

                //    if (Config.Instance().StreamEnableRapidFire)
                //    {
                //        this.SendMessage($"Cooldown has started! ({untilRapidFire} until Rapid-Fire) - Poll Failed :(");

                //        if (untilRapidFire == 1)
                //        {
                //            this.SendMessage("Rapid-Fire is coming up! Get your cheats ready! - List of all effects: https://bit.ly/gta-sa-chaos-mod");
                //        }
                //    }
                //    else
                //    {
                //        this.SendMessage($"Cooldown has started! - Poll Failed :(");
                //    }

                //    // Make sure we end poll, thank
                //    //if (activePoll != null)
                //    //{
                //    //    var response = api.Helix.Polls.EndPollAsync(UserID, activePoll.Id, TwitchLib.Api.Core.Enums.PollStatusEnum.ARCHIVED);
                //    //}
                //    //this.activePoll = null;
                //    //this.createdPoll = false;
                //}
                //else
                //{
                //    if (votingElements != null && votingElements.Count > 0)
                //    {
                //        this.SendEffectVotingToGame(false);

                //        string allEffects = string.Join(", ", votingElements.Select(e => e.GetEffect().GetDisplayName(DisplayNameType.STREAM)));

                //        if (Config.Instance().StreamEnableRapidFire)
                //        {
                //            this.SendMessage($"Cooldown has started! ({untilRapidFire} until Rapid-Fire) - Enabled effects: {allEffects}");

                //            if (untilRapidFire == 1)
                //            {
                //                this.SendMessage("Rapid-Fire is coming up! Get your cheats ready! - List of all effects: https://bit.ly/gta-sa-chaos-mod");
                //            }
                //        }
                //        else
                //        {
                //            this.SendMessage($"Cooldown has started! - Enabled effects: {allEffects}");
                //        }

                //        // Make sure we end poll, thank
                //        //if (activePoll != null)
                //        //{
                //        //    var response = api.Helix.Polls.EndPollAsync(UserID, activePoll.Id, TwitchLib.Api.Core.Enums.PollStatusEnum.ARCHIVED);
                //        //}
                //        //this.activePoll = null;
                //        //this.createdPoll = false;
                //    }
                //    else
                //    {
                //        if (Config.Instance().StreamEnableRapidFire)
                //        {
                //            this.SendMessage($"Cooldown has started! ({untilRapidFire} until Rapid-Fire)");

                //            if (untilRapidFire == 1)
                //            {
                //                this.SendMessage("Rapid-Fire is coming up! Get your cheats ready! - List of all effects: https://bit.ly/gta-sa-chaos-mod");
                //            }
                //        }
                //        else
                //        {
                //            this.SendMessage($"Cooldown has started!");
                //        }
                //    }
                //}
            });
        }

        private VoteChoice GetVoteChoice(int id)
        {
            return id switch
            {
                0 => VoteChoice.FIRST,
                1 => VoteChoice.SECOND,
                2 => VoteChoice.THIRD,
                _ => VoteChoice.NONE,
            };
        }

        public List<IVotingElement> GetVotedEffects()
        {
            this.lastChoice = VoteChoice.NONE;

            List<IVotingElement> elements = Config.Instance().StreamMajorityVotes ? this.effectVoting.GetMajorityVotes() : this.effectVoting.GetTrulyRandomVotes();
            foreach (IVotingElement e in elements)
            {
                e.GetEffect().SetSubtext($"{e.GetPercentage()}%");

                this.lastChoice |= this.GetVoteChoice(e.GetId());
            }

            return elements;
        }

        private void SendMessage(string message, bool prefix = true)
        {
            //if (this.IsConnected() && this.Channel != null && message != null)
            //{
            //    if (!this.Client.IsConnected)
            //    {
            //        this.Client.Connect();
            //        return;
            //    }

            //    if (this.Client.JoinedChannels.Count == 0)
            //    {
            //        this.Client.JoinChannel(this.Channel);
            //        return;
            //    }

            //    this.Client.SendMessage(this.Channel, $"{(prefix ? "[GTA Chaos] " : "")}{message}");
            //}
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

            if (Shared.Sync != null)
            {
                Shared.Sync.SendVotes(effects, votes, this.lastChoice, !undetermined);
            }
            else
            {
                WebsocketHandler.INSTANCE.SendVotes(effects, votes, this.lastChoice);
            }
        }

        private class ChatEffectVoting
        {
            private readonly List<ChatVotingElement> votingElements = new();
            private readonly Dictionary<string, ChatVotingElement> voters = new();

            public bool IsEmpty() => this.votingElements.Count == 0;

            public void Clear()
            {
                this.votingElements.Clear();
                this.voters.Clear();
            }

            public List<ChatVotingElement> GetVotingElements() => this.votingElements;

            public int GetTotalVotes()
            {
                int votes = 0;
                foreach (ChatVotingElement element in this.votingElements)
                {
                    votes += element.Voters.Count;
                }

                return votes;
            }

            public bool ContainsEffect(AbstractEffect effect) => this.votingElements.Any(e => e.Effect.GetDisplayName(DisplayNameType.STREAM).Equals(effect.GetDisplayName(DisplayNameType.STREAM)));

            public void AddEffect(AbstractEffect effect) => this.votingElements.Add(new ChatVotingElement(this.votingElements.Count, effect));

            public void GetVotes(out string[] effects, out int[] votes, bool undetermined = false)
            {
                ChatVotingElement[] votingElements = this.GetVotingElements().ToArray();

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
                int attempts = 0;
                while (this.votingElements.Count != possibleEffects)
                {
                    AbstractEffect effect = EffectDatabase.GetRandomEffect(true, 0, false);
                    if (effect.IsTwitchEnabled() && !this.ContainsEffect(effect))
                    {
                        this.AddEffect(effect);
                    }

                    if (attempts++ >= 10)
                    {
                        EffectDatabase.ResetEffectCooldowns();
                    }
                    else if (attempts++ >= 20)
                    {
                        break;
                    }
                }

                while (this.votingElements.Count < 3)
                {
                    AbstractEffect effect = EffectDatabase.GetRandomEffect(false, 0, false);
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
                ChatVotingElement[] elements = this.votingElements.OrderByDescending(e =>
                {
                    if (e.Voters.Count > maxVotes)
                    {
                        maxVotes = e.Voters.Count;
                    }

                    return e.Voters.Count;
                }).Where(e => e.Voters.Count == maxVotes).ToArray();

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
                foreach (ChatVotingElement e in this.votingElements)
                {
                    totalVotes += e.Voters.Count;
                }

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

                foreach (ChatVotingElement e in this.votingElements)
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
                foreach (ChatVotingElement e in this.votingElements)
                {
                    e.RemoveVoter(username);
                }

                this.votingElements[effectChoice].AddVoter(username);
                this.voters[username] = this.votingElements[effectChoice];

                foreach (ChatVotingElement element in this.votingElements)
                {
                    element.Percentage = (int)Math.Round((double)element.Voters.Count / this.GetTotalVotes() * 100);
                }
            }
        }

        public class ChatVotingElement : IVotingElement
        {
            public int Id { get; set; }

            public AbstractEffect Effect { get; set; }

            public HashSet<string> Voters { get; set; }

            public int Percentage { get; set; }

            public ChatVotingElement(int id, AbstractEffect effect)
            {
                this.Id = id;
                this.Effect = effect;
                this.Percentage = 0;
                this.Voters = new HashSet<string>();
            }

            public int GetId() => this.Id;

            public AbstractEffect GetEffect() => this.Effect;

            public int GetVotes() => this.Voters.Count;

            public int GetPercentage() => this.Percentage;

            public bool ContainsVoter(string username) => this.Voters.Contains(username);

            public void AddVoter(string username) => this.Voters.Add(username);

            public void RemoveVoter(string username) => this.Voters.Remove(username);
        }

        // Events
        public event EventHandler<EventArgs> OnConnected;
        public event EventHandler<EventArgs> OnDisconnected;
        public event EventHandler<EventArgs> OnLoginError;
        public event EventHandler<RapidFireEventArgs> OnRapidFireEffect;

        public virtual void RapidFireEffect(RapidFireEventArgs e) => OnRapidFireEffect?.Invoke(this, e);
    }
}

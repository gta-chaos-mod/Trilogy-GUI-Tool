using Flurl.Http;
using GTAChaos.Effects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GTAChaos.Utils
{
    public class YouTubeChatConnection : IStreamConnection
    {
        private class ChatItem
        {
            public string Author { get; }
            public string Message { get; }

            public ChatItem(string author, string message)
            {
                this.Author = author;
                this.Message = message;
            }
        }

        private readonly string liveId;

        private string isReplay;
        private string apiKey;
        private string clientVersion;
        private string continuation;

        private bool isConnected = false;

        private Shared.VOTING_MODE VotingMode;
        private int lastChoice = -1;
        private readonly ChatEffectVoting effectVoting = new();
        private readonly HashSet<string> rapidFireVoters = new();

        private readonly System.Timers.Timer fetchMessagesTimer;

        public YouTubeChatConnection()
        {
            this.liveId = Config.Instance().StreamAccessToken;

            if (string.IsNullOrEmpty(this.liveId))
            {
                return;
            }

            this.fetchMessagesTimer = new System.Timers.Timer()
            {
                AutoReset = true,
                Interval = 1000
            };
            this.fetchMessagesTimer.Elapsed += this.FetchMessagesTimer_Elapsed;
        }

        public async Task<bool> TryConnect()
        {
            this.Kill();

            this.isConnected = await this.FetchStreamInformation();

            if (!this.isConnected)
            {
                OnLoginError?.Invoke(this, new EventArgs());
            }
            else
            {
                OnConnected?.Invoke(this, new EventArgs());
                this.fetchMessagesTimer.Start();
            }

            return this.isConnected;
        }

        public bool IsConnected() => this.isConnected;

        private async void FetchMessagesTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!this.isConnected)
            {
                return;
            }

            List<ChatItem> messages = await this.FetchChat();
            foreach (ChatItem msg in messages)
            {
                this.OnChatMessage(msg);
            }
        }

        private string TryMatch(string data, string regex)
        {
            Match regexResult = Regex.Match(data, regex);
            return regexResult.Success ? regexResult.Groups[1].Value : null;
        }

        private async Task<bool> FetchStreamInformation()
        {
            string streamURL = $"https://www.youtube.com/watch?v={this.liveId}";

            string data = await streamURL.GetStringAsync();

            // Match all regex things
            this.isReplay = this.TryMatch(data, "['\"]isReplay['\"]:\\s*(true)");
            if (!string.IsNullOrEmpty(this.isReplay))
            {
                return false;
            }

            this.apiKey = this.TryMatch(data, "['\"]INNERTUBE_API_KEY['\"]:\\s*['\"](.+?)['\"]");
            if (string.IsNullOrEmpty(this.apiKey))
            {
                return false;
            }

            this.clientVersion = this.TryMatch(data, "['\"]clientVersion['\"]:\\s*['\"]([\\d.]+?)['\"]");
            if (string.IsNullOrEmpty(this.clientVersion))
            {
                return false;
            }

            this.continuation = this.TryMatch(data, "['\"]continuation['\"]:\\s*['\"](.+?)['\"]");
            if (string.IsNullOrEmpty(this.continuation))
            {
                return false;
            }

            Console.WriteLine(this.liveId);
            Console.WriteLine(this.isReplay);
            Console.WriteLine(this.apiKey);
            Console.WriteLine(this.clientVersion);
            Console.WriteLine(this.continuation);

            return true;
        }

        private async Task<List<ChatItem>> FetchChat()
        {
            if (!this.isConnected)
            {
                return null;
            }

            string chatURL = $"https://www.youtube.com/youtubei/v1/live_chat/get_live_chat?key={this.apiKey}";
            string data = await chatURL.PostJsonAsync(
                new
                {
                    context = new
                    {
                        client = new
                        {
                            this.clientVersion,
                            clientName = "WEB"
                        }
                    },
                    this.continuation
                }).ReceiveString();

            JObject json = JObject.Parse(data);

            JToken liveChatContinuation = json["continuationContents"]["liveChatContinuation"];

            JToken continuationData = liveChatContinuation["continuations"][0];
            if (continuationData["invalidationContinuationData"] != null)
            {
                this.continuation = continuationData["invalidationContinuationData"]["continuation"].ToString();
            }
            else if (continuationData["timedContinuationData"] != null)
            {
                this.continuation = continuationData["timedContinuationData"]["continuation"].ToString();
            }

            JToken actions = liveChatContinuation["actions"];
            return this.ParseChatMessages(actions);
        }

        private List<ChatItem> ParseChatMessages(JToken actions)
        {
            List<ChatItem> messages = new();

            if (actions == null)
            {
                return messages;
            }

            foreach (JToken action in actions)
            {
                JToken item = action["addChatItemAction"]?["item"];
                if (item == null)
                {
                    continue;
                }

                JToken renderer = null;
                if (item["liveChatTextMessageRenderer"] != null)
                {
                    renderer = item["liveChatTextMessageRenderer"];
                }
                else if (item["liveChatPaidMessageRenderer"] != null)
                {
                    renderer = item["liveChatPaidMessageRenderer"];
                }
                else if (item["liveChatMembershipItemRenderer"] != null)
                {
                    renderer = item["liveChatMembershipItemRenderer"];
                }

                if (renderer == null)
                {
                    continue;
                }

                // Message Runs
                JToken messageItem = null;
                if (renderer["message"] != null)
                {
                    messageItem = renderer["message"]?["runs"];
                }
                else if (item["headerSubtext"] != null)
                {
                    messageItem = renderer["headerSubtext"]?["runs"];
                }

                if (messageItem == null || messageItem[0] == null)
                {
                    continue;
                }

                // Author name
                string author = null;
                if (renderer["authorName"]?["simpleText"] != null)
                {
                    author = renderer["authorName"]?["simpleText"].ToString();
                }

                List<string> messageParts = new();

                JArray messageTokens = (JArray)messageItem;
                foreach (JToken msg in messageTokens)
                {
                    if (msg["text"] != null)
                    {
                        messageParts.Add(msg["text"].ToString());
                    }
                }

                string message = string.Join(" ", messageParts.ToArray());
                if (string.IsNullOrEmpty(author) || string.IsNullOrEmpty(message))
                {
                    continue;
                }

                messages.Add(new ChatItem(author, message));
            }

            return messages;
        }

        private void OnChatMessage(ChatItem chatItem)
        {
            string username = this.RemoveSpecialCharacters(chatItem.Author);
            string message = this.RemoveSpecialCharacters(chatItem.Message, true);

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
            else if (this.VotingMode == Shared.VOTING_MODE.VOTING)
            {
                int choice = this.TryParseUserChoice(message);
                if (choice is >= 0 and <= 2)
                {
                    this.effectVoting?.TryAddVote(username, choice);
                }
            }
        }

        public int GetRemaining() => 0;

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

        public void Kill() => this.fetchMessagesTimer?.Stop();

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

        private void SendMessage(string text)
        {
            // Empty method because we are just a chat listener
        }

        public void SetVoting(Shared.VOTING_MODE votingMode, int untilRapidFire = -1, List<IVotingElement> votingElements = null)
        {
            this.VotingMode = votingMode;

            if (this.VotingMode == Shared.VOTING_MODE.VOTING)
            {
                this.effectVoting.Clear();
                this.effectVoting.GenerateRandomEffects();
                this.lastChoice = -1;

                if (Config.Instance().StreamCombineChatMessages)
                {
                    string messageToSend = "Voting has started! Type 1, 2 or 3 (or #1, #2, #3) to vote for one of the effects! ";

                    foreach (ChatVotingElement element in this.effectVoting.GetVotingElements())
                    {
                        string description = element.Effect.GetDisplayName(DisplayNameType.STREAM);
                        messageToSend += $"#{element.Id + 1}: {description}. ";
                    }

                    this.SendMessage(messageToSend);
                }
                else
                {
                    this.SendMessage("Voting has started! Type 1, 2 or 3 (or #1, #2, #3) to vote for one of the effects!");

                    foreach (ChatVotingElement element in this.effectVoting.GetVotingElements())
                    {
                        string description = element.Effect.GetDisplayName(DisplayNameType.STREAM);
                        this.SendMessage($"#{element.Id + 1}: {description}");
                    }
                }
            }
            else if (this.VotingMode == Shared.VOTING_MODE.RAPID_FIRE)
            {
                this.rapidFireVoters.Clear();
                this.SendMessage("ATTENTION, ALL GAMERS! RAPID-FIRE HAS BEGUN! VALID EFFECTS WILL BE ENABLED FOR 15 SECONDS!");
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

        private string RemoveSpecialCharacters(string text, bool noSpaces = true)
        {
            string regex = noSpaces ? @"[^A-Za-z0-9]" : @"[^A-Za-z0-9 ]";
            return Regex.Replace(text, regex, "");
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
                // We aren't posting effects to YouTube chat so we can't hide them ingame
                undetermined = false;

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

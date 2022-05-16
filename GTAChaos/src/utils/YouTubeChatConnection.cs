using Flurl;
using Flurl.Http;
using GTAChaos.Effects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GTAChaos.Utils
{
    public class YouTubeChatConnection : IStreamConnection
    {
        class ChatItem
        {
            public string Author { get; }
            public string Message { get; }

            public ChatItem(string author, string message)
            {
                Author = author;
                Message = message;
            }
        }

        private readonly string liveId;

        private string isReplay;
        private string apiKey;
        private string clientVersion;
        private string continuation;

        private bool isConnected = false;

        private int VotingMode;
        private int lastChoice = -1;
        private readonly ChatEffectVoting effectVoting = new ChatEffectVoting();
        private readonly HashSet<string> rapidFireVoters = new HashSet<string>();

        private readonly System.Timers.Timer fetchMessagesTimer;

        public YouTubeChatConnection()
        {
            liveId = Config.Instance().StreamAccessToken;

            if (string.IsNullOrEmpty(liveId)) return;

            fetchMessagesTimer = new System.Timers.Timer()
            {
                AutoReset = true,
                Interval = 1000
            };
            fetchMessagesTimer.Elapsed += FetchMessagesTimer_Elapsed;
        }

        public async Task<bool> TryConnect()
        {
            Kill();

            isConnected = await FetchStreamInformation();

            if (!isConnected)
            {
                OnLoginError?.Invoke(this, new EventArgs());
            }
            else
            {
                OnConnected?.Invoke(this, new EventArgs());
                fetchMessagesTimer.Start();
            }

            return isConnected;
        }

        public bool IsConnected()
        {
            return isConnected;
        }

        private async void FetchMessagesTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!isConnected) return;

            var messages = await FetchChat();
            foreach (var msg in messages)
            {
                OnChatMessage(msg);
            }
        }

        private string TryMatch(string data, string regex)
        {
            var regexResult = Regex.Match(data, regex);
            if (regexResult.Success)
            {
                return regexResult.Groups[1].Value;
            }

            return null;
        }

        private async Task<bool> FetchStreamInformation()
        {
            string streamURL = $"https://www.youtube.com/watch?v={liveId}";

            var data = await streamURL.GetStringAsync();

            // Match all regex things
            isReplay = TryMatch(data, "['\"]isReplay['\"]:\\s*(true)");
            if (!string.IsNullOrEmpty(isReplay)) return false;

            apiKey = TryMatch(data, "['\"]INNERTUBE_API_KEY['\"]:\\s*['\"](.+?)['\"]");
            if (string.IsNullOrEmpty(apiKey)) return false;

            clientVersion = TryMatch(data, "['\"]clientVersion['\"]:\\s*['\"]([\\d.]+?)['\"]");
            if (string.IsNullOrEmpty(clientVersion)) return false;

            continuation = TryMatch(data, "['\"]continuation['\"]:\\s*['\"](.+?)['\"]");
            if (string.IsNullOrEmpty(continuation)) return false;

            Console.WriteLine(liveId);
            Console.WriteLine(isReplay);
            Console.WriteLine(apiKey);
            Console.WriteLine(clientVersion);
            Console.WriteLine(continuation);

            return true;
        }

        private async Task<List<ChatItem>> FetchChat()
        {
            if (!isConnected) return null;

            var chatURL = $"https://www.youtube.com/youtubei/v1/live_chat/get_live_chat?key={apiKey}";
            var data = await chatURL.PostJsonAsync(
                new
                {
                    context = new
                    {
                        client = new
                        {
                            clientVersion,
                            clientName = "WEB"
                        }
                    },
                    continuation
                }).ReceiveString();

            var json = JObject.Parse(data);

            var liveChatContinuation = json["continuationContents"]["liveChatContinuation"];

            var continuationData = liveChatContinuation["continuations"][0];
            if (continuationData["invalidationContinuationData"] != null)
            {
                continuation = continuationData["invalidationContinuationData"]["continuation"].ToString();
            }
            else if (continuationData["timedContinuationData"] != null)
            {
                continuation = continuationData["timedContinuationData"]["continuation"].ToString();
            }

            var actions = liveChatContinuation["actions"];
            return ParseChatMessages(actions);
        }

        private List<ChatItem> ParseChatMessages(JToken actions)
        {
            List<ChatItem> messages = new List<ChatItem>();

            if (actions == null) return messages;

            foreach(var action in actions)
            {
                var item = action["addChatItemAction"]?["item"];
                if (item == null) continue;

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

                if (renderer == null) continue;

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

                if (messageItem == null || messageItem[0] == null) continue;

                // Author name
                string author = null;
                if (renderer["authorName"]?["simpleText"] != null)
                {
                    author = renderer["authorName"]?["simpleText"].ToString();
                }

                List<string> messageParts = new List<string>();

                JArray messageTokens = (JArray) messageItem;
                foreach (var msg in messageTokens)
                {
                    if (msg["text"] != null)
                    {
                        messageParts.Add(msg["text"].ToString());
                    }
                }

                string message = string.Join(" ", messageParts.ToArray());
                if (string.IsNullOrEmpty(author) || string.IsNullOrEmpty(message)) continue;

                messages.Add(new ChatItem(author, message));
            }

            return messages;
        }

        private void OnChatMessage(ChatItem chatItem)
        {
            string username = RemoveSpecialCharacters(chatItem.Author);
            string message = RemoveSpecialCharacters(chatItem.Message, true);

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
            else if (VotingMode == 1)
            {
                int choice = TryParseUserChoice(message);
                if (choice >= 0 && choice <= 2)
                {
                    effectVoting?.TryAddVote(username, choice);
                }
            }
        }

        public int GetRemaining()
        {
            return 0;
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

        public void Kill()
        {
            fetchMessagesTimer?.Stop();
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

        private void SendMessage(string text)
        {
            // Empty method because we are just a chat listener
        }

        public void SetVoting(int votingMode, int untilRapidFire = -1, List<IVotingElement> votingElements = null)
        {
            VotingMode = votingMode;

            if (VotingMode == 1)
            {
                effectVoting.Clear();
                effectVoting.GenerateRandomEffects();
                lastChoice = -1;

                if (Config.Instance().StreamCombineChatMessages)
                {
                    string messageToSend = "Voting has started! Type 1, 2 or 3 (or #1, #2, #3) to vote for one of the effects! ";

                    foreach (ChatVotingElement element in effectVoting.GetVotingElements())
                    {
                        string description = element.Effect.GetDisplayName(DisplayNameType.STREAM);
                        messageToSend += $"#{element.Id + 1}: {description}. ";
                    }

                    SendMessage(messageToSend);
                }
                else
                {
                    SendMessage("Voting has started! Type 1, 2 or 3 (or #1, #2, #3) to vote for one of the effects!");

                    foreach (ChatVotingElement element in effectVoting.GetVotingElements())
                    {
                        string description = element.Effect.GetDisplayName(DisplayNameType.STREAM);
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

                    string allEffects = string.Join(", ", votingElements.Select(e => e.GetEffect().GetDisplayName(DisplayNameType.STREAM)));

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

        private string RemoveSpecialCharacters(string text, bool noSpaces = true)
        {
            var regex = noSpaces ? @"[^A-Za-z0-9]" : @"[^A-Za-z0-9 ]";
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
                return votingElements.Any(e => e.Effect.GetDisplayName(DisplayNameType.STREAM).Equals(effect.GetDisplayName(DisplayNameType.STREAM)));
            }

            public void AddEffect(AbstractEffect effect)
            {
                votingElements.Add(new ChatVotingElement(votingElements.Count, effect));
            }

            public void GetVotes(out string[] effects, out int[] votes, bool undetermined = false)
            {
                // We aren't posting effects to YouTube chat so we can't hide them ingame
                undetermined = false;

                ChatVotingElement[] votingElements = GetVotingElements().ToArray();

                effects = new string[]
                {
                    undetermined ? "???" : votingElements[0].Effect.GetDisplayName(DisplayNameType.STREAM),
                    undetermined ? "???" : votingElements[1].Effect.GetDisplayName(DisplayNameType.STREAM),
                    undetermined ? "???" : votingElements[2].Effect.GetDisplayName(DisplayNameType.STREAM)
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
                foreach (var e in votingElements)
                {
                    e.RemoveVoter(username);
                }

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

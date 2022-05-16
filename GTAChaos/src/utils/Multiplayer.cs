// Copyright (c) 2019 Lordmau5
using GTAChaos.Effects;
using Newtonsoft.Json;
using System;
using WebSocketSharp;

namespace GTAChaos.Utils
{
    public class ConnectionFailedEventArgs : EventArgs { }

    public class ConnectionSuccessfulEventArgs : EventArgs
    {
        public bool IsHost;
        public string HostUsername;
    }

    public class UsernameInUseEventArgs : EventArgs { }

    public class HostLeftChannelEventArgs : EventArgs { }

    public class VersionMismatchEventArgs : EventArgs
    {
        public string Version;
    }

    public class UserJoinedEventArgs : EventArgs
    {
        public string Username;
    }

    public class UserLeftEventArgs : EventArgs
    {
        public string Username;
    }

    public class ChatMessageEventArgs : EventArgs
    {
        public string Username;
        public string Message;
    }

    public class TimeUpdateEventArgs : EventArgs
    {
        public int Remaining;
        public int Total;
    }

    public class EffectEventArgs : EventArgs
    {
        public string Word;
        public int Duration;
        public string Voter;
        public int Seed;
    }

    public class VotesEventArgs : EventArgs
    {
        public string[] Effects;
        public int[] Votes;
        public int LastChoice;
    }

#pragma warning disable 0649

    internal class MessageType
    {
        /*
         * 0 = Connection successful - Extra data
         * 1 = Username in use
         * 2 = Host left the channel
         * 3 = Version Mismatch
         *
         * 10 = User Joined
         * 11 = User Left
         * 12 = Chat Message
         *
         * 20 = Time Update
         * 21 = Send Effect
         * 22 = Update Votes
         */
        public int Type;
    }

    internal class MessageConnect
    {
        public int Type = 0;
        public string Channel;
        public string Username;
        public string Version;
    }

    internal class MessageConnectionSuccessful
    {
        public int Type = 0;
        public bool IsHost;
        public string HostUsername;
    }

    internal class MessageVersionMismatch
    {
        public int Type = 3;
        public string Version;
    }

    internal class MessageUserJoined
    {
        public int Type = 10;
        public string Username;
    }

    internal class MessageUserLeft
    {
        public int Type = 11;
        public string Username;
    }

    internal class MessageChatMessage
    {
        public int Type = 12;
        public string Username;
        public string Message;
    }

    internal class MessageTimeUpdate
    {
        public int Type = 20;
        public int Remaining;
        public int Total;
    }

    internal class MessageEffect
    {
        public int Type = 21;
        public string Word;
        public int Duration;
        public string Voter;
        public int Seed;
    }

    internal class MessageVotes
    {
        public int Type = 22;
        public string[] Effects;
        public int[] Votes;
        public int LastChoice;
    }

#pragma warning restore 0649

    public class Multiplayer
    {
        public event EventHandler<ConnectionFailedEventArgs> OnConnectionFailed;

        public event EventHandler<UsernameInUseEventArgs> OnUsernameInUse;

        public event EventHandler<ConnectionSuccessfulEventArgs> OnConnectionSuccessful;

        public event EventHandler<HostLeftChannelEventArgs> OnHostLeftChannel;

        public event EventHandler<VersionMismatchEventArgs> OnVersionMismatch;

        public event EventHandler<UserJoinedEventArgs> OnUserJoined;

        public event EventHandler<UserLeftEventArgs> OnUserLeft;

        public event EventHandler<ChatMessageEventArgs> OnChatMessage;

        public event EventHandler<TimeUpdateEventArgs> OnTimeUpdate;

        public event EventHandler<EffectEventArgs> OnEffect;

        public event EventHandler<VotesEventArgs> OnVotes;

        private readonly string Channel;
        private readonly string Username;

        public bool IsHost { get; private set; }

        private bool ManualClose;
        private readonly WebSocket socket = null;

        private DateTime lastTimeUpdate;
        private DateTime lastVotesUpdate;

        public Multiplayer(string Server, string Channel, string Username)
        {
            this.Channel = Channel;
            this.Username = Username;

            this.socket = new WebSocket(Server);

            this.socket.OnOpen += (sender, e) =>
            {
                MessageConnect connect = new()
                {
                    Channel = this.Channel,
                    Username = this.Username,
                    Version = Shared.Version
                };

                this.socket.Send(JsonConvert.SerializeObject(connect));
            };

            this.socket.OnClose += (sender, e) =>
            {
                if (!this.ManualClose && !string.IsNullOrWhiteSpace(e.Reason))
                {
                    OnConnectionFailed?.Invoke(this, new ConnectionFailedEventArgs());
                }
            };

            this.socket.OnMessage += (sender, e) =>
            {
                MessageType messageType = JsonConvert.DeserializeObject<MessageType>(e.Data);
                if (messageType.Type == 0) // Connection Successful
                {
                    MessageConnectionSuccessful connectionSuccessful = JsonConvert.DeserializeObject<MessageConnectionSuccessful>(e.Data);

                    this.IsHost = connectionSuccessful.IsHost;

                    ConnectionSuccessfulEventArgs succ = new()
                    {
                        IsHost = connectionSuccessful.IsHost,
                        HostUsername = connectionSuccessful.HostUsername
                    };

                    OnConnectionSuccessful?.Invoke(this, succ);
                }
                else if (messageType.Type == 1) // Username in use
                {
                    OnUsernameInUse?.Invoke(this, new UsernameInUseEventArgs());
                    this.socket.Close();
                }
                else if (messageType.Type == 2) // Host left the channel
                {
                    OnHostLeftChannel?.Invoke(this, new HostLeftChannelEventArgs());
                    this.socket.Close();
                }
                else if (messageType.Type == 3)
                {
                    MessageVersionMismatch mismatch = JsonConvert.DeserializeObject<MessageVersionMismatch>(e.Data);

                    VersionMismatchEventArgs args = new()
                    {
                        Version = mismatch.Version
                    };

                    OnVersionMismatch?.Invoke(this, args);
                }
                // -------
                else if (messageType.Type == 10) // User Joined
                {
                    MessageUserJoined user = JsonConvert.DeserializeObject<MessageUserJoined>(e.Data);

                    UserJoinedEventArgs args = new()
                    {
                        Username = user.Username
                    };

                    OnUserJoined?.Invoke(this, args);
                }
                else if (messageType.Type == 11) // User Left
                {
                    MessageUserLeft user = JsonConvert.DeserializeObject<MessageUserLeft>(e.Data);

                    UserLeftEventArgs args = new()
                    {
                        Username = user.Username
                    };

                    OnUserLeft?.Invoke(this, args);
                }
                else if (messageType.Type == 12) // Chat Message
                {
                    MessageChatMessage chatMessage = JsonConvert.DeserializeObject<MessageChatMessage>(e.Data);

                    ChatMessageEventArgs args = new()
                    {
                        Username = chatMessage.Username,
                        Message = chatMessage.Message
                    };

                    OnChatMessage?.Invoke(this, args);
                }
                // -------
                else if (messageType.Type == 20) // Time Update
                {
                    MessageTimeUpdate timeUpdate = JsonConvert.DeserializeObject<MessageTimeUpdate>(e.Data);

                    TimeUpdateEventArgs args = new()
                    {
                        Remaining = timeUpdate.Remaining,
                        Total = timeUpdate.Total
                    };

                    OnTimeUpdate?.Invoke(this, args);
                }
                else if (messageType.Type == 21) // Send Effect
                {
                    MessageEffect effect = JsonConvert.DeserializeObject<MessageEffect>(e.Data);

                    EffectEventArgs args = new()
                    {
                        Word = effect.Word,
                        Duration = effect.Duration,
                        Voter = effect.Voter,
                        Seed = effect.Seed
                    };

                    OnEffect?.Invoke(this, args);
                }
                else if (messageType.Type == 22) // Votes
                {
                    MessageVotes votes = JsonConvert.DeserializeObject<MessageVotes>(e.Data);

                    VotesEventArgs args = new()
                    {
                        Effects = votes.Effects,
                        Votes = votes.Votes,
                        LastChoice = votes.LastChoice
                    };

                    OnVotes?.Invoke(this, args);
                }
            };
        }

        public void Connect() => this.socket?.Connect();

        public void Disconnect()
        {
            this.ManualClose = true;
            this.socket?.Close();
        }

        public void SendChatMessage(string message)
        {
            MessageChatMessage msg = new()
            {
                Username = Username,
                Message = message
            };
            this.socket?.Send(JsonConvert.SerializeObject(msg));
        }

        public void SendTimeUpdate(int remaining, int total)
        {
            DateTime now = DateTime.Now;
            if (this.lastTimeUpdate < now)
            {
                this.lastTimeUpdate = now.AddMilliseconds(500);

                MessageTimeUpdate msg = new()
                {
                    Remaining = remaining,
                    Total = total
                };
                this.socket?.Send(JsonConvert.SerializeObject(msg));
            }
        }

        public void SendEffect(AbstractEffect effect, int _duration = -1)
        {
            int duration = effect.Duration > 0 ? effect.Duration : Config.GetEffectDuration();

            if (_duration != -1)
            {
                duration = _duration; // Always Override
            }

            MessageEffect msg = new()
            {
                Word = effect.Word,
                Duration = duration,
                Voter = effect.GetVoter(),
                Seed = RandomHandler.Next(9999999)
            };
            this.socket?.Send(JsonConvert.SerializeObject(msg));
        }

        public void SendVotes(string[] effects, int[] votes, int lastChoice, bool force = false)
        {
            DateTime now = DateTime.Now;
            if (this.lastVotesUpdate < now || force)
            {
                this.lastVotesUpdate = now.AddSeconds(1);
                MessageVotes msg = new()
                {
                    Effects = effects,
                    Votes = votes,
                    LastChoice = lastChoice
                };
                this.socket?.Send(JsonConvert.SerializeObject(msg));
            }
        }
    }
}

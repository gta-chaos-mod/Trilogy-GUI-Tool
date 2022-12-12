// Copyright (c) 2019 Lordmau5
using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GTAChaos.Utils
{
    public class SocketMessageEventArgs : EventArgs
    {
        public string Data { get; set; }
    }

    public class WebsocketHandler
    {
        public static WebsocketHandler INSTANCE = new();

        public event EventHandler<SocketMessageEventArgs> OnSocketMessage;

        private WebSocketServer server;
        private readonly List<IWebSocketConnection> sockets = new();
        private readonly List<string> socketBuffer = new();

        public void ConnectWebsocket()
        {
            if (this.server is not null)
            {
                return;
            }

            this.server = new WebSocketServer("ws://0.0.0.0:9001");

            this.sockets.Clear();

            this.server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    this.sockets.Add(socket);
                    this.SendWebsocketBuffer();
                };
                socket.OnClose = () => this.sockets.Remove(socket);
                socket.OnError = error =>
                {
                    this.sockets.Remove(socket);
                    socket.Close();
                };
                socket.OnMessage = message => OnSocketMessage?.Invoke(this, new SocketMessageEventArgs { Data = message });
            });
        }

        private void SendToAllClients(string text)
        {
            foreach (IWebSocketConnection socket in this.sockets)
            {
                socket.Send(text);
            }
        }

        private void SendWebsocketBuffer()
        {
            if (this.socketBuffer.Count > 0)
            {
                foreach (string buffer in this.socketBuffer)
                {
                    this.SendToAllClients(buffer);
                }

                this.socketBuffer.Clear();
            }
        }

        public void SendDataToWebsocket(JObject jsonObject)
        {
            Task.Run(() =>
            {
                string json = JsonConvert.SerializeObject(jsonObject);

                this.ConnectWebsocket();

                if (this.sockets.Count > 0)
                {
                    this.SendWebsocketBuffer();

                    this.SendToAllClients(json);
                }
                else
                {
                    if (jsonObject["type"].ToObject<string>() != "time")
                    {
                        this.socketBuffer.Add(json);
                    }
                }
            });
        }

        public void SendTimeToGame(int remaining, int cooldown = 0, string mode = "")
        {
            JObject jsonObject = JObject.FromObject(new
            {
                type = "time",
                data = new
                {
                    remaining,
                    cooldown,
                    mode
                }
            });

            this.SendDataToWebsocket(jsonObject);
        }

        public void SendVotes(string[] effects, int[] votes, int pickedChoice = -1)
        {
            JObject jsonObject = JObject.FromObject(new
            {
                type = "votes",
                data = new
                {
                    effects,
                    votes,
                    pickedChoice
                }
            });

            this.SendDataToWebsocket(jsonObject);
        }

        public void SendEffectToGame(string effectID, object effectData = null, int duration = -1, string displayName = "", string subtext = "", bool rapidFire = false)
        {
            if (rapidFire)
            {
                duration = Math.Min(duration, 1000 * 15); // 15 seconds max. for Rapid-Fire
            }

            JObject jsonObject = JObject.FromObject(new
            {
                type = "effect",
                data = new
                {
                    effectID,
                    effectData = effectData ?? (new { }),
                    duration,
                    displayName = this.IsNullOrEmpty(displayName) ? effectID : displayName,
                    subtext
                }
            });

            this.SendDataToWebsocket(jsonObject);
        }

        private bool IsNullOrEmpty(string value) => value == null || value.Length == 0;
    }
}

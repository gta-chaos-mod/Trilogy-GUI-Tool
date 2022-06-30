// Copyright (c) 2019 Lordmau5
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WebSocketSharp;

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

        private WebSocket socket;
        private bool socketIsConnecting = false;
        private bool socketConnected = false;
        private readonly List<string> socketBuffer = new();

        public void ConnectWebsocket()
        {
            try
            {
                if (!this.socketConnected && !this.socketIsConnecting)
                {
                    this.socket = new WebSocket("ws://localhost:9001");
                    this.socket.OnOpen += this.Socket_OnOpen;
                    this.socket.OnClose += this.Socket_OnClose;
                    this.socket.OnError += this.Socket_OnError;
                    this.socket.OnMessage += this.Socket_OnMessage;

                    this.socketIsConnecting = true;

                    this.socket.Connect();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);

                this.socketConnected = false;
                this.socketIsConnecting = false;
            }
        }

        private void Socket_OnMessage(object sender, MessageEventArgs e)
        {
            if (!e.IsText)
            {
                return;
            }

            OnSocketMessage?.Invoke(this, new SocketMessageEventArgs { Data = e.Data });
        }

        private void Socket_OnOpen(object sender, EventArgs e)
        {
            this.socketConnected = true;
            this.socketIsConnecting = false;

            this.SendWebsocketBuffer();
        }

        private void Socket_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            this.socketConnected = false;
            this.socketIsConnecting = false;
        }

        private void Socket_OnClose(object sender, CloseEventArgs e)
        {
            this.socketConnected = false;
            this.socketIsConnecting = false;
        }

        private void SendWebsocketBuffer()
        {
            if (this.socketBuffer.Count > 0)
            {
                foreach (string buffer in this.socketBuffer)
                {
                    this.socket?.Send(buffer);
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

                if (this.socketConnected)
                {
                    this.SendWebsocketBuffer();

                    this.socket?.Send(json);
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
                duration = 1000 * 15; // 15 seconds for Rapid-Fire
            }

            JObject jsonObject = JObject.FromObject(new
            {
                type = "effect",
                data = new
                {
                    effectID,
                    effectData = effectData ?? (new { }),
                    duration,
                    displayName = displayName.IsNullOrEmpty() ? effectID : displayName,
                    subtext
                }
            });

            // TODO: Implement JSON data in Crowd Control like this
            /*
            if (crowdControlID != -1)
            {
                jsonObject["data"]["crowdControlData"] = JObject.FromObject(new
                {
                    id = crowdControlID,
                    buyer = "Joshimuz"
                });
            }
            */

            this.SendDataToWebsocket(jsonObject);
        }
    }
}

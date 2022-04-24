// Copyright (c) 2019 Lordmau5
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebSocketSharp;

namespace GtaChaos.Models.Utils
{
    public class SocketMessageEventArgs : EventArgs
    {
        public string Data { get; set; }
    }

    public class WebsocketHandler
    {
        public static WebsocketHandler INSTANCE = new WebsocketHandler();

        public event EventHandler<SocketMessageEventArgs> OnSocketMessage;

        private WebSocket socket;
        private bool socketIsConnecting = false;
        private bool socketConnected = false;
        private readonly List<string> socketBuffer = new List<string>();

        public void ConnectWebsocket()
        {
            try
            {
                if (!socketConnected && !socketIsConnecting)
                {
                    socket = new WebSocket("ws://localhost:9001");
                    socket.OnOpen += Socket_OnOpen;
                    socket.OnClose += Socket_OnClose;
                    socket.OnError += Socket_OnError;
                    socket.OnMessage += Socket_OnMessage;

                    socketIsConnecting = true;

                    socket.Connect();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                socketConnected = false;
                socketIsConnecting = false;
            }
        }

        private void Socket_OnMessage(object sender, MessageEventArgs e)
        {
            if (!e.IsText) return;

            OnSocketMessage?.Invoke(this, new SocketMessageEventArgs { Data = e.Data });
        }

        private void Socket_OnOpen(object sender, EventArgs e)
        {
            socketConnected = true;
            socketIsConnecting = false;
        }

        private void Socket_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            socketConnected = false;
            socketIsConnecting = false;
        }

        private void Socket_OnClose(object sender, CloseEventArgs e)
        {
            socketConnected = false;
            socketIsConnecting = false;
        }

        public void SendDataToWebsocket(JObject jsonObject)
        {
            Task.Run(() =>
            {
                string json = JsonConvert.SerializeObject(jsonObject);

                ConnectWebsocket();

                if (socketConnected)
                {
                    if (socketBuffer.Count > 0)
                    {
                        foreach (string buffer in socketBuffer)
                        {
                            socket?.Send(buffer);
                        }

                        socketBuffer.Clear();
                    }

                    socket?.Send(json);
                }
                else
                {
                    if (jsonObject["type"].ToObject<string>() != "time")
                    {
                        socketBuffer.Add(json);
                    }
                }
            });
        }

        public void SendTimeToGame(int remaining, int cooldown = 0, string mode = "")
        {
            var jsonObject = JObject.FromObject(new
            {
                type = "time",
                data = new
                {
                    remaining,
                    cooldown,
                    mode
                }
            });

            SendDataToWebsocket(jsonObject);
        }

        public void SendVotes(string[] effects, int[] votes, int pickedChoice = -1)
        {
            var jsonObject = JObject.FromObject(new
            {
                type = "votes",
                data = new
                {
                    effects,
                    votes,
                    pickedChoice
                }
            });

            SendDataToWebsocket(jsonObject);
        }

        public void SendEffectToGame(string effectID, object effectData = null, int duration = -1, string displayName = "", string twitchVoter = "", bool rapidFire = false)
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
                    displayName = displayName.IsNullOrEmpty() ? effectID : displayName
                }
            });

            if (!twitchVoter.IsNullOrEmpty())
            {
                jsonObject["data"]["twitchData"] = JObject.FromObject(new
                {
                    voter = twitchVoter
                    // TODO: effectPercentage = XYZ (so it can show "Voter (32%)" ingame perhaps - Issue 106 on GitHub)
                });
            }

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

            SendDataToWebsocket(jsonObject);
        }
    }
}

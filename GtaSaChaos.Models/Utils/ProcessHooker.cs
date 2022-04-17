// Copyright (c) 2019 Lordmau5
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;
using WebSocketSharp;

namespace GtaChaos.Models.Utils
{
    // TODO: Rework as singleton (public static readonly ProcessHooker INSTANCE = new ProcessHooker();
    public static class ProcessHooker
    {
        private static WebSocket socket;
        private static bool socketConnected = false;
        private static List<string> socketBuffer = new List<string>();
        private static Process Process = null;

        public static void HookProcess()
        {
            CloseProcess();

            Process = Process.GetProcessesByName("gta_sa").FirstOrDefault();
            if (Process == null)
            {
                Process = Process.GetProcessesByName("gta-sa").FirstOrDefault();
            }
            if (Process == null)
            {
                return;
            }

            Process.EnableRaisingEvents = true;
        }

        public static IntPtr GetHandle()
        {
            return Process == null ? IntPtr.Zero : Process.Handle;
        }

        private static void ConnectWebsocket()
        {
            try
            {
                if (socket == null)
                {
                    socket = new WebSocket("ws://localhost:9001");
                    socket.OnOpen += Socket_OnOpen;
                    socket.OnClose += Socket_OnClose;
                    socket.OnError += Socket_OnError;

                    socket.Connect();
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        private static void Socket_OnOpen(object sender, EventArgs e)
        {
            socketConnected = true;
        }

        private static void Socket_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            socketConnected = false;
            socket = null;
        }

        private static void Socket_OnClose(object sender, CloseEventArgs e)
        {
            socketConnected = false;
            socket = null;
        }

        public static void SendDataToWebsocket(JObject jsonObject)
        {
            Task.Run(() =>
            {
                string json = JsonConvert.SerializeObject(jsonObject);

                if (true)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(jsonObject, Formatting.Indented));
                }

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
                        //socketBuffer.Add(json);
                    }
                }
            });
        }

        public static void SendTimeToGame(int remaining, int cooldown = 0, string mode = "")
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

        public static void SendVotes(string[] effects, int[] votes, int pickedChoice = -1)
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

        public static void SendEffectToGame(string effectID, object effectData = null, int duration = -1, string displayName = "", string twitchVoter = "", bool rapidFire = false)
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

        public static void AttachExitedMethod(EventHandler method)
        {
            if (Process != null)
            {
                Process.Exited += method;
            }
        }

        public static bool HasExited()
        {
            return Process == null || Process.HasExited;
        }

        public static void CloseProcess()
        {
            try
            {
                Process = null;
            }
            catch { }
        }
    }
}

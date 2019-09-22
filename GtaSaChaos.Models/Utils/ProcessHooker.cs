// Copyright (c) 2019 Lordmau5

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;

namespace GtaChaos.Models.Utils
{
    public static class ProcessHooker
    {
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

        public static void SendPipeMessage(string func)
        {
            new System.Threading.Thread(() =>
            {
                System.Threading.Thread.CurrentThread.IsBackground = true;

                using (NamedPipeClientStream pipeStream = new NamedPipeClientStream("GTASAChaosPipe"))
                {
                    try
                    {
                        if (!pipeStream.IsConnected)
                            pipeStream.Connect(1000);

                        using (StreamWriter sw = new StreamWriter(pipeStream))
                        {
                            if (sw.AutoFlush == false)
                                sw.AutoFlush = true;
                            sw.WriteLine(func);
                        }
                    }
                    catch
                    {
                        // Timeouts are okay, don't log anything
                    }
                }
            }).Start();
        }

        public static void SendEffectToGame(string type, string function, int duration = -1, string description = "N/A", string voter = "N/A", int rapidfire = 0)
        {
            string builtString = $"{type}:{function}:{duration}:{description}:{voter}:{rapidfire}";
            SendPipeMessage(builtString);
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

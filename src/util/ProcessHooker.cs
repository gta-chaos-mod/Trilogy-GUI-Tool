using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;

namespace GTA_SA_Chaos
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
            using (NamedPipeClientStream pipeStream = new NamedPipeClientStream("GTASAChaosPipe"))
            {
                try
                {
                    if (!pipeStream.IsConnected)
                        pipeStream.Connect(250);

                    using (StreamWriter sw = new StreamWriter(pipeStream))
                    {
                        if (sw.AutoFlush == false)
                            sw.AutoFlush = true;
                        sw.WriteLine(func);
                    }
                }
                catch { }
            }
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
